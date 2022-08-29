using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MusicExpanded.Patches
{
    public class MusicManagerEntry
    {
        public static FieldInfo audioSourceField = AccessTools.Field(typeof(RimWorld.MusicManagerEntry), "audioSource");
        public static MethodInfo startPlaying = AccessTools.Method(typeof(RimWorld.MusicManagerEntry), "StartPlaying");
        [HarmonyPatch(typeof(RimWorld.MusicManagerEntry), "StartPlaying")]
        class StartPlaying
        {
            static bool Prefix(RimWorld.MusicManagerEntry __instance)
            {
                ThemeDef.ResolveSounds();
                AudioSource audioSource = audioSourceField.GetValue(__instance) as AudioSource;
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                    return false;
                }
                if (GameObject.Find("MusicAudioSourceDummy") != null)
                {
                    Log.Error("MusicManagerEntry did StartPlaying but there is already a music source GameObject.");
                    return false;
                }
                // Figure out the menu track to play
                SongDef menuSong = Utilities.GetTrack(Cue.MainMenu) as SongDef;
                // Fall back if there is none
                if (menuSong == null)
                    menuSong = SongDefOf.EntrySong;
                // Play track
                GameObject gameObject = new GameObject("MusicAudioSourceDummy");
                gameObject.transform.parent = Camera.main.transform;
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.bypassEffects = true;
                audioSource.bypassListenerEffects = true;
                audioSource.bypassReverbZones = true;
                audioSource.priority = 0;
                audioSource.clip = menuSong.clip;
                audioSource.volume = __instance.CurSanitizedVolume;
                audioSource.loop = true;
                audioSource.spatialBlend = 0f;
                audioSource.Play();
                audioSourceField.SetValue(__instance, audioSource);
                return false;
            }
        }
    }
}