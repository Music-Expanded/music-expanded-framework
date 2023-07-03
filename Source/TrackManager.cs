using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MusicExpanded
{
    // This class manages the enabled music tracks, adding and removing tracks, and finding appropriate tracks to play
    public static class TrackManager
    {
        // This list stores the music tracks
        public static List<TrackDef> tracks = new List<TrackDef>();

        // Returns all the tracks with named colonists.
        public static IEnumerable<TrackDef> TracksWithNamedColonist => tracks.Where(track => track.cue == Cue.HasColonistNamed);

        // Returns the track with the specified definition name.
        public static TrackDef TrackByDefName(string defName) => tracks.Find(track => track.defName == defName);

        // Returns all the tracks with the specified cue and data.
        public static IEnumerable<TrackDef> TracksByCue(Cue cue, string data = null)
        {
            // Select all tracks that have the specified cue and data.
            IEnumerable<TrackDef> selectedTracks = tracks.Where(track =>
            {
                if (!data.NullOrEmpty() && track.cueData != data) return false;
                // Only return what's appropriate
                return track.AppropriateNow(null, cue);
            });
            return selectedTracks;
        }

        // Returns a random track with the specified cue and data.
        public static TrackDef GetTrack(Cue cue, string cueData = null)
        {
            // Select all tracks with the specified cue and data, and choose one randomly based on its commonality.
            TracksByCue(cue, cueData).TryRandomElementByWeight((TrackDef s) => s.commonality, out TrackDef track);
            return track;
        }

        // Adds all the tracks from the specified theme to the list of tracks.
        public static void Add(ThemeDef theme)
        {
            foreach (TrackDef track in theme.tracks)
            {
                tracks.Add(track);
            }
        }

        // Removes all the tracks from the specified theme from the list of tracks.
        public static void Remove(ThemeDef theme)
        {
            tracks.RemoveAll(track => theme.tracks.Contains(track));
        }

        // This method initializes the music tracks, by adding tracks from enabled themes, as well as creating the vanilla theme.
        public static void Init()
        {
            // Generate the vanilla theme.
            ThemeDef.GenerateVanillaTheme();
            try {
                // Add tracks from all the enabled themes to the list of tracks.
                foreach (ThemeDef theme in DefDatabase<ThemeDef>.AllDefsListForReading)
                {
                    if (Core.settings.enabledThemes.TryGetValue(theme.defName, false))
                    {
                        Add(theme);
                    }
                }
            } catch {}
        }
    }
}
