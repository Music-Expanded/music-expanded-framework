using HarmonyLib;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace MusicExpanded.Patches
{
    public class Scenario
    {
        // This Harmony patch runs after a scenario has finished loading.
        [HarmonyPatch(typeof(RimWorld.Scenario), "PostGameStart")]
        class PostGameStart
        {
            static void Postfix(RimWorld.Scenario __instance)
            {
                // Get the player's map. If there is no player home map, use the current map.
                Map map = Find.AnyPlayerHomeMap ?? Find.CurrentMap;
                
                // Get a list of pawns on the map that are relevant to the storyteller.
                List<Pawn> pawns = map.PlayerPawnsForStoryteller.ToList();
                
                // If there is only one pawn, play the LoneColonist track.
                if (pawns.Count() == 1)
                {
                    Utilities.PlayTrack(Cue.LoneColonist);
                    return;
                }
                else
                {
                    // Otherwise, get all tracks with a named colonist, and try to play the track matching each pawn's name.
                    IEnumerable<TrackDef> tracks = TrackManager.TracksWithNamedColonist;
                    foreach (Pawn pawn in pawns)
                        if (Utilities.PlayTrack(tracks.Where(track => Utilities.NameMatches(pawn, track.cueData))))
                            return;
                }
            }

        }
    }
}
