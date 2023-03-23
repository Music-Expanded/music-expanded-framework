using System.Collections.Generic;
using HarmonyLib;
using Verse;
using Verse.Sound;

namespace MusicExpanded
{
    public static class SoundManager
    {
        // Dictionary to store vanilla sub-sounds
        private static Dictionary<string, List<SubSoundDef>> vanillaSubSounds = new Dictionary<string, List<SubSoundDef>>();

        // Initialize the SoundManager
        public static void Init()
        {
            ActivateSounds(Core.settings.SoundTheme);
        }

        // Activate sounds for a given theme
        public static void ActivateSounds(ThemeDef theme)
        {
            // Get a list of all vanilla SoundDefs
            List<Verse.SoundDef> vanillaSoundDefs = DefDatabase<Verse.SoundDef>.AllDefsListForReading;

            // Reset all sounds before activating new sounds
            ResetSounds(vanillaSoundDefs);

            // Loop through all vanilla SoundDefs and replace sub-sounds with sub-sounds from the given theme
            foreach (Verse.SoundDef vanillaSound in vanillaSoundDefs)
            {
                // Find the SoundDef in the theme that replaces this vanilla SoundDef
                SoundDef expandedSound = theme.sounds.Find(sound => sound.GetModExtension<ModExtension.ReplacesSounds>().sounds.Contains(vanillaSound));

                // If the SoundDef is not found in the theme, continue to the next SoundDef
                if (expandedSound == null) continue;

                // Store the original sub-sounds for the vanilla SoundDef in the dictionary
                vanillaSubSounds.SetOrAdd(vanillaSound.defName, vanillaSound.subSounds);

                // Replace the sub-sounds for the vanilla SoundDef with the sub-sounds from the theme's SoundDef
                vanillaSound.subSounds = expandedSound.subSounds;

                // Resolve references for the new sub-sounds
                vanillaSound.ResolveReferences();
            }
        }

        // Reset all sounds to their original sub-sounds
        private static void ResetSounds(List<Verse.SoundDef> vanillaSoundDefs)
        {
            // Loop through all vanilla SoundDefs and restore their original sub-sounds
            foreach (Verse.SoundDef vanillaSound in vanillaSoundDefs)
            {
                // Get the original sub-sounds from the dictionary
                List<SubSoundDef> subSounds = vanillaSubSounds.GetValueSafe(vanillaSound.defName);

                // If the original sub-sounds are not found in the dictionary, continue to the next SoundDef
                if (subSounds == null) continue;

                // Replace the sub-sounds for the vanilla SoundDef with the original sub-sounds
                vanillaSound.subSounds = subSounds;

                // Resolve references for the original sub-sounds
                vanillaSound.ResolveReferences();

                // Remove the original sub-sounds from the dictionary
                vanillaSubSounds.Remove(vanillaSound.defName);
            }
        }
    }
}
