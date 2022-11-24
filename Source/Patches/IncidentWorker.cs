using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace MusicExpanded.Patches
{
    [HarmonyPatch(typeof(RimWorld.IncidentWorker), "TryExecute")]
    public class IncidentWorker
    {
        static void Postfix(RimWorld.IncidentWorker __instance, IncidentParms parms, ref bool __result)
        {
            if (__result != true) return;
            ModExtension.PlayCue playCue = __instance.def.GetModExtension<ModExtension.PlayCue>();
            if (playCue == null) return;
            if (playCue.playBattleTrack)
                Utilities.PlayTrack(Utilities.BattleCue(parms.points));
            else
                Utilities.PlayTrack(playCue.cue, playCue.cueData);
        }
    }
}