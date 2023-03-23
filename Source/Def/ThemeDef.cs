using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MusicExpanded
{
    public class ThemeDef : Def
    {
        public List<TrackDef> tracks;
        public List<SoundDef> sounds = new List<SoundDef>();
        public string iconPath;

        // Get the method info for the GiveShortHash method in the ShortHashGiver class
        private static MethodInfo giveShortHash = AccessTools.Method(typeof(Verse.ShortHashGiver), "GiveShortHash");

        // Define a static property called VanillaTheme which returns the ThemeDef with the name "ME_Vanilla"
        public static ThemeDef VanillaTheme => DefDatabase<ThemeDef>.GetNamedSilentFail("ME_Vanilla");

        // Define a static method called GenerateVanillaTheme which creates a new ThemeDef for the vanilla game
        public static void GenerateVanillaTheme()
        {
            // Check if a vanilla theme already exists
            ThemeDef vanillaTheme = DefDatabase<ThemeDef>.GetNamedSilentFail("ME_Vanilla");
            if (vanillaTheme != null) return;

            // If it doesn't, create a new ThemeDef object and set its properties
            vanillaTheme = new ThemeDef();
            vanillaTheme.label = "ME_Vanilla_Label".Translate();
            vanillaTheme.description = "ME_Vanilla_Description".Translate();
            vanillaTheme.defName = "ME_Vanilla";
            vanillaTheme.tracks = new List<TrackDef>();
            vanillaTheme.iconPath = "UI/HeroArt/RimWorldLogo";

            // Get all SongDefs that do not have corresponding TrackDefs and create TrackDefs for them
            IEnumerable<SongDef> songs = DefDatabase<SongDef>.AllDefsListForReading.Where((SongDef track) =>
            {
                return DefDatabase<TrackDef>.GetNamedSilentFail(track.defName) == null;
            });

            // Get a set of hash codes for all existing TrackDefs
            HashSet<ushort> trackHashes = Utilities.GetHashes(typeof(TrackDef));

            // For each SongDef without a corresponding TrackDef, create a new TrackDef, set its properties, and add it to the vanilla theme's list of tracks
            foreach (SongDef song in songs)
            {
                TrackDef track = TrackDef.FromSong(song);
                track.defName = "ME_Vanilla_" + song.defName;
                giveShortHash.Invoke(null, new object[] { track, typeof(TrackDef), trackHashes });
                vanillaTheme.tracks.Add(track);
            }

            // Get a set of hash codes for all existing ThemeDefs
            HashSet<ushort> themeHashes = Utilities.GetHashes(typeof(ThemeDef));

            // Give the vanilla theme a short hash code and add it to the DefDatabase
            giveShortHash.Invoke(null, new object[] { vanillaTheme, typeof(ThemeDef), themeHashes });
            DefDatabase<ThemeDef>.Add(vanillaTheme);
        }

        // Define a public method called Preview which plays a random song from the theme
        public void Preview()
        {
            try
            {
                MusicManagerPlay manager = Find.MusicManagerPlay; // get the current music manager (used in gameplay)
                if (manager != null && manager.IsPlaying)
                    manager.ForceStartSong(tracks.RandomElement() as SongDef, false); // start playing a random track from the theme's tracks list
            }
            catch
            {
                // if the above code fails (e.g. the game is not in gameplay mode), use the entry music manager instead
                MusicManagerEntry manager = Find.MusicManagerEntry;
                AudioSource audioSource = Patches.MusicManagerEntry.audioSourceField.GetValue(manager) as AudioSource; // get the audio source of the entry music manager

                SongDef menuSong = tracks.Where(track => track.cue == Cue.MainMenu).RandomElement() as SongDef; // get a random track with the cue "MainMenu" from the theme's tracks list
                if (menuSong == null) // if no tracks with the cue "MainMenu" were found, use the default entry song
                    menuSong = SongDefOf.EntrySong;

                audioSource.clip = menuSong.clip; // set the audio clip of the audio source to the selected track
                audioSource.Stop(); // stop playing any previous music

                Patches.MusicManagerEntry.startPlaying.Invoke(manager, null); // start playing the new track
            }
        }
    }
}
