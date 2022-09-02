using HarmonyLib;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace MusicExpanded.Patches
{
    public class Scenario
    {
        [HarmonyPatch(typeof(RimWorld.Scenario), "PostGameStart")]
        class PostGameStart
        {
            static void Postfix(RimWorld.Scenario __instance)
            {
                Map map = Find.AnyPlayerHomeMap ?? Find.CurrentMap;
                List<Pawn> pawns = map.PlayerPawnsForStoryteller.ToList();
                if (pawns.Count() == 1)
                {
                    Utilities.PlayTrack(Cue.LoneColonist);
                    return;
                }
                else
                {
                    IEnumerable<TrackDef> tracks = ThemeDef.TracksWithNamedColonist;
                    foreach (Pawn pawn in pawns)
                        if (Utilities.PlayTrack(tracks.Where(track => Utilities.NameMatches(pawn, track.cueData))))
                            return;
                }
            }

        }
    }
}