using HarmonyLib;
using RimWorld;

namespace MusicExpanded.Patches
{
    // Harmony patch to intercept the TryExecute method in RimWorld.IncidentWorker class
    [HarmonyPatch(typeof(RimWorld.IncidentWorker), nameof(RimWorld.IncidentWorker.TryExecute))]
    public class IncidentWorker
    {
        // Postfix method to be executed after TryExecute
        static void Postfix(RimWorld.IncidentWorker __instance, IncidentParms parms, ref bool __result)
        {
            // If the incident was not successfully executed, do not continue
            if (__result != true) return;

            // Get the PlayCue mod extension from the incident definition
            ModExtension.PlayCue playCue = __instance.def.GetModExtension<ModExtension.PlayCue>();

            // If there is no PlayCue mod extension, do not continue
            if (playCue == null) return;

            // If the incident should play a battle track, play a battle track
            if (playCue.playBattleTrack)
                Utilities.PlayTrack(Utilities.BattleCue(parms.points));

            // Otherwise, play the specified cue and cueData
            else
                Utilities.PlayTrack(playCue.cue, playCue.cueData);
        }
    }
}
