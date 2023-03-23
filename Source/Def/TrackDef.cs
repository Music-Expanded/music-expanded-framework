using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace MusicExpanded
{
    public class TrackDef : SongDef
    {
        // Whether to use the vanilla logic for determining if this track is appropriate to play.
        public bool vanillaLogic = false;

        // Cue to play this track on.
        public Cue cue = Cue.None;

        // Additional cue data for HasColonistNamed cue.
        public string cueData;

        // MethodInfo for the vanilla appropriateNow method, used for comparison.
        public static MethodInfo vanillaAppropriateNow = AccessTools.Method(typeof(RimWorld.MusicManagerPlay), "AppropriateNow");

        // Whether this is a battle track.
        public bool IsBattleTrack => (cue <= Cue.BattleLegendary && cue >= Cue.BattleSmall);

        // Allowed biomes for this track.
        public List<BiomeDef> allowedBiomes;

        // Allowed tech levels for this track.
        public List<TechLevel> allowedTechLevels;

        // Returns whether this track is appropriate to play now, given the provided last played song and cue.
        public bool AppropriateNow(SongDef lastPlayed = null, Cue cueMatch = Cue.None)
        {
            // Checks if the conditions for this track are not met, returning false if any fail.
            if (
                (cue == Cue.HasColonistNamed && Find.CurrentMap != null && !Find.CurrentMap.PlayerPawnsForStoryteller.Where((pawn) => Utilities.NameMatches(pawn, cueData)).Any())
                || (cue != cueMatch || (lastPlayed != null && lastPlayed == this))
                || (allowedBiomes != null && Find.CurrentMap != null && !allowedBiomes.Contains(Find.CurrentMap.Biome))
                || (allowedTechLevels != null && !allowedTechLevels.Contains(Find.FactionManager.OfPlayer.def.techLevel))
            )
                return false;

            // If vanillaLogic is set, use vanilla appropriateNow logic.
            if (vanillaLogic)
                return (bool)vanillaAppropriateNow.Invoke(Find.MusicManagerPlay, new SongDef[] { this as SongDef });

            // Otherwise, return true.
            return true;
        }

        // Returns a new TrackDef from the provided SongDef.
        public static TrackDef FromSong(SongDef song)
        {
            TrackDef track = new TrackDef();

            // Set basic properties.
            track.defName = song.defName;
            track.label = song.defName.Replace("_", " ");
            track.description = song.description;
            track.generated = true;

            // Set song properties.
            track.clipPath = song.clipPath;
            track.volume = song.volume;
            track.playOnMap = song.playOnMap;
            track.commonality = song.commonality;
            track.tense = song.tense;
            track.allowedTimeOfDay = song.allowedTimeOfDay;
            track.allowedSeasons = song.allowedSeasons;
            track.clip = song.clip;

            // Since this track is generated via vanilla SongDef, it feels safe to assume it'll use vanillaLogic.
            track.vanillaLogic = true;

            return track;
        }
    }
}
