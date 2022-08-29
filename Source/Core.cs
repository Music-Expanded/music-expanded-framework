using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace MusicExpanded
{
    public class Core : Mod
    {
        public static Settings settings;
        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public Core(ModContentPack content) : base(content)
        {
            settings = GetSettings<Settings>();
            var harmony = new Harmony("musicexpanded.core");
            harmony.PatchAll();
        }
        public override string SettingsCategory()
        {
            return "ME_MusicExpanded".Translate();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {

            Listing_Standard outerListing = new Listing_Standard();
            outerListing.Begin(inRect);
            Listing_Standard checkboxListing = new Listing_Standard();
            Rect checkboxRect = outerListing.GetRect(30f).LeftHalf();
            checkboxListing.Begin(checkboxRect);
            checkboxListing.CheckboxLabeled("ME_ShowNowPlaying".Translate(), ref settings.showNowPlaying, "ME_ShowNowPlayingDescription".Translate());
            checkboxListing.End();

            List<ThemeDef> themes = DefDatabase<ThemeDef>.AllDefs.ToList();

            // The height of individual ThemeDef entries. 
            float entryHeight = 140f;
            viewHeight = entryHeight * themes.Count() + 40f;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 18f, viewHeight);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect, true);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);
            foreach (ThemeDef theme in themes)
            {
                ThemeWidget(theme, listing.GetRect(entryHeight));
            }

            listing.End();
            Widgets.EndScrollView();

            outerListing.End();

        }

        private void ThemeWidget(ThemeDef themeDef, Rect mainRect)
        {
            float height = mainRect.height;
            if (themeDef.defName == settings.selectedTheme)
            {
                Widgets.DrawHighlight(mainRect);
            }
            Rect iconRect = mainRect;
            iconRect.xMax -= mainRect.width - height;
            iconRect.xMin = iconRect.xMax - height;

            Rect textRect = mainRect;
            textRect.xMin += height + 5f;
            textRect.xMax -= 5f;
            textRect.yMin += 5f;

            if (!themeDef.iconPath.NullOrEmpty())
            {
                Texture2D icon = ContentFinder<Texture2D>.Get(themeDef.iconPath, true);
                Widgets.DrawTextureFitted(iconRect, icon, 1);
            }

            Listing_Standard textListing = new Listing_Standard();
            textListing.Begin(textRect);

            Text.Font = GameFont.Medium;
            textListing.Label(themeDef.label);
            Text.Font = GameFont.Small;
            textListing.Label(themeDef.description);

            Rect selectButtonRect = textListing.GetRect(30f).LeftPartPixels(150f);

            Listing_Standard selectButtonListing = new Listing_Standard();
            selectButtonListing.Begin(selectButtonRect);
            if (selectButtonListing.ButtonText("ME_SelectTheme".Translate()))
                ThemeDef.Select(themeDef);
            selectButtonListing.End();

            textListing.End();
        }

    }
}
