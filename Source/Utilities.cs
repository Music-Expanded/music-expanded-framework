using Verse;
using System.Linq;
using System.Collections.Generic;
using RimWorld;

namespace MusicExpanded
{
    public static class Utilities
    {
        public static TrackDef GetTrack(Cue cue, string name = null)
        {
            ThemeDef.TracksByCue(cue, name).TryRandomElementByWeight((TrackDef s) => s.commonality, out TrackDef track);
            return track;
        }
        public static bool PlayTrack(Cue cue, string name = null) => PlayTrack(ThemeDef.TracksByCue(cue, name));
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
                Messages.Message("ME_NowPlaying".Translate(song.label).ToString(), null, MessageTypeDefOf.NeutralEvent, null, false);
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
    }
}
