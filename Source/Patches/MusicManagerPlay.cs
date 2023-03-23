using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MusicExpanded.Patches
{
    public class MusicManagerPlay
    {
        // Declare some static fields with reflection info
        public static MethodInfo startNewSong = AccessTools.Method(typeof(RimWorld.MusicManagerPlay), "StartNewSong");
        public static FieldInfo gameObjectCreated = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "gameObjectCreated");
        public static FieldInfo forcedSong = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "forcedNextSong");
        public static FieldInfo lastStartedSong = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "lastStartedSong");
        public static FieldInfo ignorePrefsVolumeThisSong = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "ignorePrefsVolumeThisSong");
        public static FieldInfo audioSource = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "audioSource");

        // Patch RimWorld.MusicManagerPlay.ChooseNextSong with Harmony
        [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "ChooseNextSong")]
        class ChooseNextSong
        {
            // Prefix patch to replace the original method
            static bool Prefix(RimWorld.MusicManagerPlay __instance, ref SongDef __result)
            {
                // Check if "RunInBackground" is disabled and "vanillaMusicUpdate" is false
                if (!Verse.Prefs.RunInBackground && !Core.settings.vanillaMusicUpdate)
                {
                    // Log an error message and set "vanillaMusicUpdate" to true for compatibility
                    Log.Error("Using Music Expanded Framework while RimWorld's setting \"RunInBackground\" is disabled is known to cause issues! Enabling \"vanillaMusicUpdate\" setting in Music Expanded Framework settings to ensure proper compatibility. If you'd like to use our custom update, enable RimWorld's \"Run In Background\" setting, and disable Music Expanded Frameworks's \"Vanilla Music Update\"");
                    Core.settings.vanillaMusicUpdate = true;
                }
                // Get the forced song
                System.Object forcedSong = MusicManagerPlay.forcedSong.GetValue(__instance);
                // If there is a forced song, show it and return
                if (forcedSong != null)
                {
                    Utilities.ShowNowPlaying(forcedSong as SongDef);
                    return true;
                }
                // Get the last started song
                SongDef lastTrack = MusicManagerPlay.lastStartedSong.GetValue(__instance) as SongDef;
                IEnumerable<TrackDef> tracks = null;

                // Battle track decay
                if (lastTrack != null)
                {
                    TrackDef lastTrackAsTrackDef = TrackManager.TrackByDefName(lastTrack.defName);
                    if (lastTrackAsTrackDef != null && lastTrackAsTrackDef.IsBattleTrack)
                    {
                        Map map = Find.AnyPlayerHomeMap ?? Find.CurrentMap;
                        if (map.dangerWatcher.DangerRating == StoryDanger.High)
                        {
                            Cue battleCue = (Cue)(lastTrackAsTrackDef.cue - 1);
                            if (battleCue != Cue.None)
                                tracks = TrackManager.TracksByCue(battleCue);
                        }
                    }
                }

                // If no tracks were found for battle track decay, get appropriate tracks from all tracks
                if (tracks == null || !tracks.Any())
                {
                    tracks = TrackManager.tracks.Where(track => track.AppropriateNow(lastTrack));
                }
                // if there's still no tracks were found, then enable the vanilla theme
                if (!tracks.Any())
                {
                    Log.Warning("Couldn't find a track to play, enabling Vanilla Theme to (hopefully) avoid this in the future. Playing a random track from all possible tracks.");
                    Core.settings.Enable(ThemeDef.VanillaTheme);
                    tracks = TrackManager.tracks;
                }
                // Randomly select a track based on its commonality (weighted probability)
                SongDef chosenTrack = tracks.RandomElementByWeight((TrackDef s) => s.commonality) as SongDef;

                // Display the currently playing track in the debug log
                Utilities.ShowNowPlaying(chosenTrack);

                // Set the result to the chosen track and return false to prevent the original method from executing
                __result = chosenTrack;
                return false;
            }
        }

        // This patch handles the end credits song, ensuring that it is always the correct track
        [HarmonyPatch(typeof(Screen_Credits), "EndCreditsSong", MethodType.Getter)]
        class EndCreditsSong
        {
            static bool Prefix(ref SongDef __result)
            {
                // Get the track associated with the end credits cue
                TrackDef track = TrackManager.GetTrack(Cue.Credits);

                // If the track is found, set the result to the associated SongDef and return false to prevent the original method from executing
                if (track != null)
                {
                    __result = track as SongDef;
                    return false;
                }

                // Otherwise, return true to allow the original method to execute
                return true;
            }
        }

        // This patch handles the music update process
        [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "MusicUpdate")]
        class MusicUpdate
        {
            static bool Prefix(RimWorld.MusicManagerPlay __instance)
            {
                // Get the value of the gameObjectCreated field
                bool gameObjectCreated = (bool)MusicManagerPlay.gameObjectCreated.GetValue(__instance);

                // If vanillaMusicUpdate is enabled, gameObjectCreated is false, or the MusicManagerPlay is disabled, return true to allow the original method to execute
                if (Core.settings.vanillaMusicUpdate || !gameObjectCreated || __instance.disabled)
                    return true;

                try
                {
                    // If no song is currently playing, set ignorePrefsVolumeThisSong to false and start a new song
                    if (!__instance.IsPlaying)
                    {
                        MusicManagerPlay.ignorePrefsVolumeThisSong.SetValue(__instance, false);
                        startNewSong.Invoke(__instance, null);
                    }
                    // Otherwise, update the volume of the audio source to match the current sanitized volume
                    else
                    {
                        AudioSource audioSource = (AudioSource)MusicManagerPlay.audioSource.GetValue(__instance);
                        audioSource.volume = __instance.CurSanitizedVolume;
                    }
                }
                catch
                {
                    // If an exception is thrown, log a warning
                    Log.Warning("Couldn't start a new song");
                }

                // Return false to prevent the original method from executing
                return false;
            }
        }
        // Solves this issue: https://github.com/Music-Expanded/music-expanded-framework/issues/56
        // Essentially, if a track is forced to play before the game has begun, don't try to play it immediately, but store it.
        [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "ForceStartSong")]
        class ForceStartSong
        {
            static bool Prefix(RimWorld.MusicManagerPlay __instance, SongDef song)
            {
                // Get the value of the gameObjectCreated field
                bool gameObjectCreated = (bool)MusicManagerPlay.gameObjectCreated.GetValue(__instance);

                // Set the forcedSong field to the specified song
                MusicManagerPlay.forcedSong.SetValue(__instance, song);

                // If gameObjectCreated is false, return true to allow
                return !(!gameObjectCreated);
            }
        }
    }
}