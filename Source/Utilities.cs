using Verse;
using System.Linq;
using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using System;

namespace MusicExpanded
{
    public static class Utilities
    {
        // Play a random track from the given cue
        public static bool PlayTrack(Cue cue, string cueData = null) => PlayTrack(TrackManager.TracksByCue(cue, cueData));

        // Play a random track from the given list of tracks
        public static bool PlayTrack(IEnumerable<TrackDef> tracks)
        {
            if (!tracks.Any()) // if there are no tracks to play, return false
                return false;
            // choose a random track weighted by commonality
            tracks.TryRandomElementByWeight((TrackDef s) => s.commonality, out TrackDef track); \
            // start playing the chosen track
            Find.MusicManagerPlay.ForceStartSong(track as SongDef, false); 
            return true;
        }

        // Show a message with the currently playing song
        public static void ShowNowPlaying(SongDef song)
        {
            // if the mod's settings allow showing the now playing message
            if (Core.settings.showNowPlaying) 
                Messages.Message("ME_NowPlaying".Translate(song.label).ToString(), null, MessageTypeDefOf.SilentInput, null, false); // show the message
        }

        // Get the appropriate battle cue for the given battle points
        public static Cue BattleCue(float points)
        {
            if (points > 5000) // if there are over 5000 points
                return Cue.BattleLegendary; // return the legendary battle cue
            if (points > 2500) // if there are over 2500 points
                return Cue.BattleLarge; // return the large battle cue
            if (points > 500) // if there are over 500 points
                return Cue.BattleMedium; // return the medium battle cue
            return Cue.BattleSmall; // otherwise return the small battle cue
        }

        // Check if the pawn's name contains the given substring
        public static bool NameMatches(Pawn pawn, string name)
        {
            // check if the lowercase name contains the lowercase substring
            return pawn.Name.ToStringFull.ToLower().Contains(name.ToLower()); 
        }

        // Get the hashes already taken by the given type. This is particularly useful for ensuring the hashes don't collide when creating new defs dynamically.
        public static HashSet<ushort> GetHashes(Type type)
        {
            HashSet<ushort> hashset;
            try
            {
                hashset = ((Dictionary<Type, HashSet<ushort>>)AccessTools.Field(typeof(ShortHashGiver), "takenHashesPerDeftype").GetValue(null))[type];
            }
            catch
            {
                hashset = new HashSet<ushort>();
            }
            return hashset;
        }
    }
}
