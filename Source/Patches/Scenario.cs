using HarmonyLib;
using Verse;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MusicExpanded.Patches
{
    public class Scenario
    {
        [HarmonyPatch(typeof(RimWorld.Scenario), "PostGameStart")]
        class PostGameStart
        {
            static bool NameMatches(Pawn pawn, string name)
            {
                return pawn.Name.ToStringFull.ToLower().Contains(name.ToLower());
            }
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
                        if (Utilities.PlayTrack(tracks.Where(track => NameMatches(pawn, track.namedPawn))))
                            return;
                }
            }

        }
    }
}