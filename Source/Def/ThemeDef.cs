using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MusicExpanded
{
    public class ThemeDef : Def
    {
        public List<TrackDef> tracks;
        public List<SoundDef> sounds = new List<SoundDef>();
        public string iconPath;
        private static MethodInfo giveShortHash = AccessTools.Method(typeof(Verse.ShortHashGiver), "GiveShortHash");
        public static ThemeDef VanillaTheme => DefDatabase<ThemeDef>.GetNamedSilentFail("ME_Vanilla");
        public static void GenerateVanillaTheme()
        {
            ThemeDef vanillaTheme = DefDatabase<ThemeDef>.GetNamedSilentFail("ME_Vanilla");
            if (vanillaTheme != null) return;

            vanillaTheme = new ThemeDef();
            vanillaTheme.label = "ME_Vanilla_Label".Translate();
            vanillaTheme.description = "ME_Vanilla_Description".Translate();
            vanillaTheme.defName = "ME_Vanilla";
            vanillaTheme.tracks = new List<TrackDef>();
            vanillaTheme.iconPath = "UI/HeroArt/RimWorldLogo";
            IEnumerable<SongDef> songs = DefDatabase<SongDef>.AllDefsListForReading.Where((SongDef track) =>
            {
                return DefDatabase<TrackDef>.GetNamedSilentFail(track.defName) == null;
            });
            HashSet<ushort> trackHashes = Utilities.GetHashes(typeof(TrackDef));
            foreach (SongDef song in songs)
            {
                TrackDef track = TrackDef.FromSong(song);
                track.defName = "ME_Vanilla_" + song.defName;
                giveShortHash.Invoke(null, new object[] { track, typeof(TrackDef), trackHashes });
                vanillaTheme.tracks.Add(track);
            }
            HashSet<ushort> themeHashes = Utilities.GetHashes(typeof(ThemeDef));
            giveShortHash.Invoke(null, new object[] { vanillaTheme, typeof(ThemeDef), themeHashes });
            DefDatabase<ThemeDef>.Add(vanillaTheme);

        }
        public void Preview()
        {
            try
            {
                MusicManagerPlay manager = Find.MusicManagerPlay;
                if (manager != null && manager.IsPlaying)
                    manager.ForceStartSong(tracks.RandomElement() as SongDef, false);
            }
            catch
            {
                MusicManagerEntry manager = Find.MusicManagerEntry;
                AudioSource audioSource = Patches.MusicManagerEntry.audioSourceField.GetValue(manager) as AudioSource;

                SongDef menuSong = tracks.Where(track => track.cue == Cue.MainMenu).RandomElement() as SongDef;
                if (menuSong == null)
                    menuSong = SongDefOf.EntrySong;

                audioSource.clip = menuSong.clip;
                audioSource.Stop();

                Patches.MusicManagerEntry.startPlaying.Invoke(manager, null);
            }
        }
    }
}
