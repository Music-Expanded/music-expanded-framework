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
        public static ThemeDef ActiveTheme => DefDatabase<ThemeDef>.GetNamed(Core.settings.selectedTheme);
        public List<TrackDef> tracks;
        public List<SoundDef> sounds = new List<SoundDef>();
        public string iconPath;
        public static IEnumerable<TrackDef> TracksWithNamedColonist => ActiveTheme.tracks.Where(track => track.cue == Cue.StartWithNamedColonist);
        public static TrackDef TrackByDefName(string defName) => ActiveTheme.tracks.Find(track => track.defName == defName);
        public static IEnumerable<TrackDef> TracksByCue(Cue cue, string name = null)
        {
            return ActiveTheme.tracks.Where(track =>
            {
                return track.cue == cue && (!name.NullOrEmpty() || name == track.namedPawn);
            });
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
                SoundDef expandedSound = theme.sounds.Find(sound => sound.replaces.Contains(vanillaSound));
                if (expandedSound == null) continue;
                Log.Message("Replacing " + vanillaSound.defName);
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
