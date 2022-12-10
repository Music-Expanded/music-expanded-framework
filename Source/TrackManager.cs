using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MusicExpanded
{
    public static class TrackManager
    {
        public static List<TrackDef> tracks;
        public static IEnumerable<TrackDef> TracksWithNamedColonist => tracks.Where(track => track.cue == Cue.HasColonistNamed);
        public static TrackDef TrackByDefName(string defName) => tracks.Find(track => track.defName == defName);
        public static IEnumerable<TrackDef> TracksByCue(Cue cue, string data = null)
        {
            IEnumerable<TrackDef> selectedTracks = tracks.Where(track =>
            {
                if (!data.NullOrEmpty() && track.cueData != data) return false;
                return track.AppropriateNow(null, cue);
            });
            return selectedTracks;
        }
        public static TrackDef GetTrack(Cue cue, string cueData = null)
        {
            TracksByCue(cue, cueData).TryRandomElementByWeight((TrackDef s) => s.commonality, out TrackDef track);
            return track;
        }
        public static void Select(ThemeDef theme)
        {
            // Add tracks from the selected theme to the list of tracks
            tracks = (List<TrackDef>)(tracks.Concat(theme.tracks));
        }
        public static void Remove(ThemeDef theme)
        {
            tracks.RemoveAll(track => theme.tracks.Contains(track));
        }
    }
}
