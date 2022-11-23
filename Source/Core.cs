using System.Collections.Generic;
using System.Linq;
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
            var harmony = new Harmony("musicexpanded.framework");
            harmony.PatchAll();
        }
        public override string SettingsCategory() => "ME_MusicExpanded".Translate();
        public override void DoSettingsWindowContents(Rect inRect) => settings.Build(inRect);
    }
}
