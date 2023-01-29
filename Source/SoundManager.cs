using System.Collections.Generic;
using HarmonyLib;
using Verse;
using Verse.Sound;

namespace MusicExpanded
{
    public static class SoundManager
    {
        private static Dictionary<string, List<SubSoundDef>> vanillaSubSounds = new Dictionary<string, List<SubSoundDef>>();
        public static void Init()
        {
            ActivateSounds(Core.settings.SoundTheme);
        }
        public static void ActivateSounds(ThemeDef theme)
        {
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
