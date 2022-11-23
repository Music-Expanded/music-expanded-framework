using System.Linq;
using UnityEngine;
using Verse;

namespace MusicExpanded
{

    public class Settings : ModSettings
    {
        public string selectedTheme = "ME_Vanilla";
        public bool showNowPlaying = true;
        private static Vector2 scrollPosition = Vector2.zero;
        private static Rect settingsContainer;
        private static float ThemeSelectionHeight = 180f;
        private static float ThemeSelectorHeight =>
            DefDatabase<ThemeDef>.AllDefsListForReading.Count() * ThemeSelectionHeight;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref selectedTheme, "selectedTheme", "ME_Vanilla");
            Scribe_Values.Look(ref showNowPlaying, "showNowPlaying", true);
        }
        public void Build(Rect container)
        {
            settingsContainer = container;
            Listing_Standard list = new Listing_Standard();
            list.Begin(container);
            BuildNowPlaying(list);
            BuildThemeSelector(list);
            list.End();
        }
        private void BuildNowPlaying(Listing_Standard container)
        {
            Listing_Standard checkboxListing = new Listing_Standard();
            Rect checkboxRect = container.GetRect(30f).LeftHalf();
            checkboxListing.Begin(checkboxRect);
            checkboxListing.CheckboxLabeled("ME_ShowNowPlaying".Translate(), ref showNowPlaying, "ME_ShowNowPlayingDescription".Translate());
            checkboxListing.End();
        }
        private void BuildThemeSelector(Listing_Standard list)
        {
            Rect viewRect = new Rect(0f, 0f, settingsContainer.width - 18f, ThemeSelectorHeight);
            Widgets.BeginScrollView(
                list.GetRect(settingsContainer.height - list.CurHeight),
                ref scrollPosition,
                viewRect
            );
            Listing_Standard themeList = new Listing_Standard();
            themeList.Begin(viewRect);
            foreach (ThemeDef theme in DefDatabase<ThemeDef>.AllDefsListForReading)
            {
                BuildThemeWidget(theme, themeList.GetRect(ThemeSelectionHeight));
            }
            themeList.End();
            Widgets.EndScrollView();
        }
        private void BuildThemeWidget(ThemeDef theme, Rect container)
        {
            if (theme.defName == selectedTheme)
                Widgets.DrawHighlight(container);

            if (Widgets.ButtonInvisible(container))
                ThemeDef.Select(theme);

            // Render Icon
            if (!theme.iconPath.NullOrEmpty())
            {
                Rect iconRect = container;
                iconRect.xMax -= container.width - container.height;
                iconRect.xMin = iconRect.xMax - container.height;
                Texture2D icon = ContentFinder<Texture2D>.Get(theme.iconPath, true);
                Widgets.DrawTextureFitted(iconRect, icon, 1);
            }

            // Render Info
            Rect textRect = container;
            textRect.xMin += container.height + 5f;
            textRect.xMax -= 5f;
            textRect.yMin += 5f;

            Listing_Standard textListing = new Listing_Standard();
            textListing.Begin(textRect);

            Text.Font = GameFont.Medium;
            textListing.Label(theme.label);
            Text.Font = GameFont.Small;
            textListing.Label(theme.description);

            textListing.End();
        }
    }
}