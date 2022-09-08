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
        public static ThemeDef ActiveTheme
        {
            get
            {
                ThemeDef theme = DefDatabase<ThemeDef>.GetNamedSilentFail(Core.settings.selectedTheme);
                if (theme == null)
                {
                    Log.Warning("Couldn't find selected theme: " + Core.settings.selectedTheme + ", defaulting to vanilla theme.");
                    theme = DefDatabase<ThemeDef>.GetNamedSilentFail("ME_Vanilla");
                    Core.settings.selectedTheme = theme.defName;
                    ThemeDef.ResolveSounds();
                }
                return theme;
            }
        }
        public List<TrackDef> tracks;
        public List<SoundDef> sounds = new List<SoundDef>();
        public string iconPath;
        public static IEnumerable<TrackDef> TracksWithNamedColonist => ActiveTheme.tracks.Where(track => track.cue == Cue.HasColonistNamed);
        public static TrackDef TrackByDefName(string defName) => ActiveTheme.tracks.Find(track => track.defName == defName);
        public static IEnumerable<TrackDef> TracksByCue(Cue cue, string data = null)
        {
            IEnumerable<TrackDef> tracks = ActiveTheme.tracks.Where(track =>
            {
                if (!data.NullOrEmpty() && track.cueData != data) return false;
                return track.AppropriateNow(null, cue);
            });
            return tracks;
        }
        private static Dictionary<string, List<SubSoundDef>> vanillaSubSounds = new Dictionary<string, List<SubSoundDef>>();
        private static MethodInfo giveShortHash = AccessTools.Method(typeof(Verse.ShortHashGiver), "GiveShortHash");
        public static void Select(ThemeDef theme)
        {
            Core.settings.selectedTheme = theme.defName;
            ThemeDef.ResolveSounds();

            try
            {
                MusicManagerPlay manager = Find.MusicManagerPlay;
                if (manager != null && manager.IsPlaying)
                {
                    Patches.MusicManagerPlay.startNewSong.Invoke(manager, null);
                }
            }
            catch
            {
                MusicManagerEntry manager = Find.MusicManagerEntry;
                AudioSource audioSource = Patches.MusicManagerEntry.audioSourceField.GetValue(manager) as AudioSource;

                SongDef menuSong = Utilities.GetTrack(Cue.MainMenu) as SongDef;
                if (menuSong == null)
                    menuSong = SongDefOf.EntrySong;

                audioSource.clip = menuSong.clip;

                Patches.MusicManagerEntry.startPlaying.Invoke(manager, null);
            }
        }
        public static void ResolveSounds(ThemeDef theme = null)
        {
            GenerateVanillaTheme();
            if (theme == null) theme = ActiveTheme;
            List<Verse.SoundDef> vanillaSoundDefs = DefDatabase<Verse.SoundDef>.AllDefsListForReading;
            ResetSounds(vanillaSoundDefs);
            foreach (Verse.SoundDef vanillaSound in vanillaSoundDefs)
            {
                SoundDef expandedSound = theme.sounds.Find(sound => sound.GetModExtension<ModExtension.ReplacesSounds>().sounds.Contains(vanillaSound));
                if (expandedSound == null) continue;
                vanillaSubSounds.SetOrAdd(vanillaSound.defName, vanillaSound.subSounds);
                vanillaSound.subSounds = expandedSound.subSounds;
                vanillaSound.ResolveReferences();
            }
        }
        private static void GenerateVanillaTheme()
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
            foreach (SongDef song in songs)
            {
                TrackDef track = TrackDef.FromSong(song);
                track.defName = "ME_Vanilla_" + song.defName;
                giveShortHash.Invoke(null, new object[] { track, typeof(TrackDef) });
                vanillaTheme.tracks.Add(track);
            }
            giveShortHash.Invoke(null, new object[] { vanillaTheme, typeof(ThemeDef) });
            DefDatabase<ThemeDef>.Add(vanillaTheme);

        }
        private static void ResetSounds(List<Verse.SoundDef> vanillaSoundDefs)
        {
            foreach (Verse.SoundDef vanillaSound in vanillaSoundDefs)
            {
                List<SubSoundDef> subSounds = vanillaSubSounds.GetValueSafe(vanillaSound.defName);
                if (subSounds == null) continue;
                vanillaSound.subSounds = subSounds;
                vanillaSound.ResolveReferences();
                vanillaSubSounds.Remove(vanillaSound.defName);
            }
        }
    }
}
