using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MusicExpanded.Patches
{
    public class MusicManagerEntry
    {
        // FieldInfo for accessing the "audioSource" private field in MusicManagerEntry
        public static FieldInfo audioSourceField = AccessTools.Field(typeof(RimWorld.MusicManagerEntry), "audioSource");

        // MethodInfo for the original "StartPlaying" method in MusicManagerEntry
        public static MethodInfo startPlaying = AccessTools.Method(typeof(RimWorld.MusicManagerEntry), "StartPlaying");

        // Harmony patch to prefix the "StartPlaying" method in MusicManagerEntry
        [HarmonyPatch(typeof(RimWorld.MusicManagerEntry), "StartPlaying")]
        class StartPlaying
        {
            static bool Prefix(RimWorld.MusicManagerEntry __instance)
            {
                // Call the static Init method in Core to ensure that MusicManagerEntry is properly initialized
                Core.Init();

                // Get the AudioSource field value from the MusicManagerEntry instance
                AudioSource audioSource = audioSourceField.GetValue(__instance) as AudioSource;

                // If the AudioSource is not null and is not already playing, play it
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                    return false;
                }

                // If there is already a music source GameObject, log an error and return
                if (GameObject.Find("MusicAudioSourceDummy") != null)
                {
                    Log.Error("MusicManagerEntry did StartPlaying but there is already a music source GameObject.");
                    return false;
                }

                // Figure out the menu track to play
                SongDef menuSong = TrackManager.GetTrack(Cue.MainMenu) as SongDef;

                // Fall back if there is none
                if (menuSong == null)
                    menuSong = SongDefOf.EntrySong;

                // Play the menu track using a new AudioSource GameObject
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

                // Set the AudioSource field value in the MusicManagerEntry instance to the new AudioSource
                audioSourceField.SetValue(__instance, audioSource);

                // Return false to prevent the original method from running
                return false;
            }
        }
    }
}
