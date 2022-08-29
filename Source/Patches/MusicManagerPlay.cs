using System;
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
        [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "ChooseNextSong")]
        class ChooseNextSong
        {
            static bool Prefix(RimWorld.MusicManagerPlay __instance, ref SongDef __result)
            {
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
                    TrackDef lastTrackAsTrackDef = ThemeDef.TrackByDefName(lastTrack.defName);
                    if (lastTrackAsTrackDef != null && lastTrackAsTrackDef.IsBattleTrack)
                    {

                        Map map = Find.AnyPlayerHomeMap ?? Find.CurrentMap;
                        if (map.dangerWatcher.DangerRating == StoryDanger.High)
                        {
                            Cue battleCue = (Cue)(lastTrackAsTrackDef.cue - 1);
                            if (battleCue != Cue.None)
                                tracks = ThemeDef.TracksByCue(battleCue);
                        }
                    }
                }

                if (tracks == null || !tracks.Any())
                {
                    tracks = ThemeDef.ActiveTheme.tracks.Where(track => track.AppropriateNow(__instance, lastTrack));
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
                TrackDef track = Utilities.GetTrack(Cue.Credits);
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
                if (!gameObjectCreated || __instance.IsPlaying || __instance.disabled)
                    return true;
                try
                {
                    startNewSong.Invoke(__instance, null);
                }
                catch
                {
                    Log.Warning("Couldn't start a new song");
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "ForceStartSong")]
        class ForceStartSong
        {
            static bool Prefix(RimWorld.MusicManagerPlay __instance, SongDef song)
            {
                bool gameObjectCreated = (bool)MusicManagerPlay.gameObjectCreated.GetValue(__instance);
                MusicManagerPlay.forcedSong.SetValue(__instance, song);
                return !(!gameObjectCreated);
            }
        }
    }
}