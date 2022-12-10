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
        public static MethodInfo startNewSong = AccessTools.Method(typeof(RimWorld.MusicManagerPlay), "StartNewSong");
        public static FieldInfo gameObjectCreated = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "gameObjectCreated");
        public static FieldInfo forcedSong = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "forcedNextSong");
        public static FieldInfo lastStartedSong = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "lastStartedSong");
        public static FieldInfo ignorePrefsVolumeThisSong = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "ignorePrefsVolumeThisSong");
        public static FieldInfo audioSource = AccessTools.Field(typeof(RimWorld.MusicManagerPlay), "audioSource");
        [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "ChooseNextSong")]
        class ChooseNextSong
        {
            static bool Prefix(RimWorld.MusicManagerPlay __instance, ref SongDef __result)
            {
                if (!Verse.Prefs.RunInBackground && !Core.settings.vanillaMusicUpdate)
                {
                    Log.Error("Using Music Expanded Framework while RimWorld's setting \"RunInBackground\" is disabled is known to cause issues! Enabling \"vanillaMusicUpdate\" setting in Music Expanded Framework settings to ensure proper compatibility. If you'd like to use our custom update, enable RimWorld's \"Run In Background\" setting, and disable Music Expanded Frameworks's \"Vanilla Music Update\"");
                    Core.settings.vanillaMusicUpdate = true;
                }
                System.Object forcedSong = MusicManagerPlay.forcedSong.GetValue(__instance);
                if (forcedSong != null)
                {
                    Utilities.ShowNowPlaying(forcedSong as SongDef);
                    return true;
                }
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

                if (tracks == null || !tracks.Any())
                {
                    tracks = TrackManager.tracks.Where(track => track.AppropriateNow(lastTrack));
                }
                if (!tracks.Any())
                {
                    Log.Warning("Tried to play a track from the theme, but none were appropriate right now. This theme requires more tracks.");
                    return false;
                }
                SongDef chosenTrack = tracks.RandomElementByWeight((TrackDef s) => s.commonality) as SongDef;
                Utilities.ShowNowPlaying(chosenTrack);
                __result = chosenTrack;
                return false;
            }
        }
        [HarmonyPatch(typeof(Screen_Credits), "EndCreditsSong", MethodType.Getter)]
        class EndCreditsSong
        {
            static bool Prefix(ref SongDef __result)
            {
                TrackDef track = TrackManager.GetTrack(Cue.Credits);
                if (track != null)
                {
                    __result = track as SongDef;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "MusicUpdate")]
        class MusicUpdate
        {
            static bool Prefix(RimWorld.MusicManagerPlay __instance)
            {
                bool gameObjectCreated = (bool)MusicManagerPlay.gameObjectCreated.GetValue(__instance);
                if (Core.settings.vanillaMusicUpdate || !gameObjectCreated || __instance.disabled)
                    return true;
                try
                {
                    if (!__instance.IsPlaying)
                    {
                        MusicManagerPlay.ignorePrefsVolumeThisSong.SetValue(__instance, false);
                        startNewSong.Invoke(__instance, null);
                    }
                    else
                    {
                        AudioSource audioSource = (AudioSource)MusicManagerPlay.audioSource.GetValue(__instance);
                        audioSource.volume = __instance.CurSanitizedVolume;
                    }
                }
                catch
                {
                    Log.Warning("Couldn't start a new song");
                }
                return false;
            }
        }
    }
}