using Verse;
using System.Linq;
using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using System;

namespace MusicExpanded
{
    public static class Utilities
    {
        public static bool PlayTrack(Cue cue, string cueData = null) => PlayTrack(TrackManager.TracksByCue(cue, cueData));
        public static bool PlayTrack(IEnumerable<TrackDef> tracks)
        {
            if (!tracks.Any())
                return false;
            tracks.TryRandomElementByWeight((TrackDef s) => s.commonality, out TrackDef track);
            Find.MusicManagerPlay.ForceStartSong(track as SongDef, false);
            return true;
        }
        public static void ShowNowPlaying(SongDef song)
        {
            if (Core.settings.showNowPlaying)
                Messages.Message("ME_NowPlaying".Translate(song.label).ToString(), null, MessageTypeDefOf.SilentInput, null, false);
        }
        public static Cue BattleCue(float points)
        {
            if (points > 5000)
                return Cue.BattleLegendary;
            if (points > 2500)
                return Cue.BattleLarge;
            if (points > 500)
                return Cue.BattleMedium;
            return Cue.BattleSmall;
        }
        public static bool NameMatches(Pawn pawn, string name)
        {
            return pawn.Name.ToStringFull.ToLower().Contains(name.ToLower());
        }
        public static HashSet<ushort> GetHashes(Type type)
        {
            HashSet<ushort> hashset;
            try
            {
                hashset = ((Dictionary<Type, HashSet<ushort>>)AccessTools.Field(typeof(ShortHashGiver), "takenHashesPerDeftype").GetValue(null))[type];
            }
            catch
            {
                hashset = new HashSet<ushort>();
            }
            return hashset;
        }
    }
}
