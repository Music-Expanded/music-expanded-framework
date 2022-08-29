
using Verse;

namespace MusicExpanded
{

    public class Settings : ModSettings
    {
        public string selectedTheme = "ME_Glitterworld";
        public bool showNowPlaying = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref selectedTheme, "selectedTheme", "ME_Glitterworld");
            Scribe_Values.Look(ref showNowPlaying, "showNowPlaying", true);
        }
    }
}