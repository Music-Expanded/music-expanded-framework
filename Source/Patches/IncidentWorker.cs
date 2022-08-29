using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using System.Linq;

namespace MusicExpanded.Patches
{
    [HarmonyPatch]
    public class IncidentWorker
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return AccessTools.AllTypes()
                .Where(type => type.IsSubclassOf(typeof(RimWorld.IncidentWorker)))
                .SelectMany(type => type.GetMethods())
                .Where(method => method.Name == "TryExecute");
        }
        static void Postfix(RimWorld.IncidentWorker __instance, IncidentParms parms, ref bool __result)
        {
            if (__result != true) return;
            ModExtension.PlayCue playCue = __instance.def.GetModExtension<ModExtension.PlayCue>();
            if (playCue == null) return;
            if (playCue.playBattleTrack)
                Utilities.PlayTrack(Utilities.BattleCue(parms.points));
            else
                Utilities.PlayTrack(playCue.cue);
        }
    }
}