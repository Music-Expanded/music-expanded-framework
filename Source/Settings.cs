using System.Linq;
using UnityEngine;
using Verse;

namespace MusicExpanded
{

    public class Settings : ModSettings
    {
        public string selectedTheme = "ME_Vanilla";
        public bool showNowPlaying = true;
        public bool vanillaMusicUpdate = false;
        private static Vector2 scrollPosition = Vector2.zero;
        private static Rect settingsContainer;
        private static float ThemeSelectionHeight = 180f;
        private static float ThemeSelectorHeight =>
            DefDatabase<ThemeDef>.AllDefsListForReading.Count() * ThemeSelectionHeight;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref selectedTheme, "selectedTheme", "ME_Vanilla");
            Scribe_Values.Look(ref showNowPlaying, "showNowPlaying", true);
            Scribe_Values.Look(ref vanillaMusicUpdate, "vanillaMusicUpdate", false);
        }
        public void Build(Rect container)
        {
            settingsContainer = container;
            Listing_Standard list = new Listing_Standard();
            list.Begin(container);

            Rect checkboxRow = list.GetRect(30f);
            BuildNowPlaying(checkboxRow.LeftHalf());
            BuildVanillaMusicUpdate(checkboxRow.RightHalf());
            BuildThemeSelector(list);
            list.End();
        }
        private void BuildNowPlaying(Rect container)
        {
            Listing_Standard checkboxListing = new Listing_Standard();
            checkboxListing.Begin(container);
            checkboxListing.CheckboxLabeled("ME_ShowNowPlaying".Translate(), ref showNowPlaying, "ME_ShowNowPlayingDescription".Translate());
            checkboxListing.End();
        }
        private void BuildVanillaMusicUpdate(Rect container)
        {
            Listing_Standard checkboxListing = new Listing_Standard();
            checkboxListing.Begin(container);
            checkboxListing.CheckboxLabeled("ME_VanillaMusicUpdate".Translate(), ref vanillaMusicUpdate, "ME_VanillaMusicUpdateDescription".Translate());
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
            Text.Font = GameFont.Medium;
            list.Label("Available Themes");
            Listing_Standard themeList = new Listing_Standard();
            themeList.Begin(viewRect);
            foreach (ThemeDef theme in DefDatabase<ThemeDef>.AllDefsListForReading)
            {
                BuildThemeWidgetNew(theme, themeList.GetRect(ThemeSelectionHeight));
            }
            themeList.End();
            Widgets.EndScrollView();
        }
        private void BuildThemeWidgetNew(ThemeDef theme, Rect container)
        {

            if (Widgets.ButtonInvisible(container))
                theme.Preview();

            Rect iconRect = container.LeftPartPixels(ThemeSelectionHeight);
            iconRect.y += 5;
            iconRect.yMax -= 5;
            if (!theme.iconPath.NullOrEmpty())
            {
                Texture2D icon = ContentFinder<Texture2D>.Get(theme.iconPath, true);
                Widgets.DrawTextureFitted(iconRect, icon, 1);
            }
            Rect rightSide = container.RightPartPixels(container.width - iconRect.width - 18f);
            // Rect labelArea = rightSide.TopPart(.60f);
            Rect buttonArea = rightSide.BottomPartPixels(20);
            Text.Font = GameFont.Medium;
            Widgets.Label(rightSide, theme.label);
            // Rect leftTwix = buttonArea.LeftPart(.5f);
            Rect rightTwix = buttonArea.RightPart(.5f);
            Text.Font = GameFont.Small;
            rightSide.y += 25;
            Widgets.Label(rightSide, theme.description);
            // ThemeButton("Info", leftTwix.LeftPart(.5f));
            // ThemeButton("Preview", leftTwix.RightPart(.5f));
            ThemeButton("Enable Theme", rightTwix.LeftPart(.5f));
            ThemeButton("Enable Sounds", rightTwix.RightPart(.5f));
            Widgets.DrawLineHorizontal(container.x, container.y + container.height, container.width);
        }
        private bool ThemeButton(string text, Rect container)
        {
            container.xMax -= 10f;
            return Widgets.ButtonText(container, text);
        }
        private void BuildThemeWidget(ThemeDef theme, Rect container)
        {
            if (theme.defName == selectedTheme)
                Widgets.DrawHighlight(container);

            if (Widgets.ButtonInvisible(container))
                TrackManager.Select(theme);

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