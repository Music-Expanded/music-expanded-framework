using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MusicExpanded
{
    public class TrackDef : SongDef
    {
        public List<BiomeDef> allowedBiomes;
        public bool playDuringBattles = false;
        public bool playOnMainMenu = false;
        public bool playOnCredits = false;
        public bool vanillaLogic = false;
        public Cue cue = Cue.None;
        public string namedPawn;
        public static MethodInfo vanillaAppropriateNow = AccessTools.Method(typeof(RimWorld.MusicManagerPlay), "AppropriateNow");
        public bool IsBattleTrack => (cue <= Cue.BattleLegendary && cue >= Cue.BattleSmall);
        public bool AppropriateNow(MusicManagerPlay manager, SongDef lastPlayed)
        {
            if (
                lastPlayed == this
                || cue != Cue.None
            )
                return false;

            if (vanillaLogic)
                return (bool)vanillaAppropriateNow.Invoke(manager, new SongDef[] { this as SongDef });

            return true;
        }
    }
}
