using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace MusicExpanded
{
    public class Core : Mod
    {
        public static Settings settings;

        public Core(ModContentPack content) : base(content)
        {
            settings = GetSettings<Settings>();
            var harmony = new Harmony("musicexpanded.framework");
            harmony.PatchAll();
        }
        public override string SettingsCategory() => "ME_MusicExpanded".Translate();
        public override void DoSettingsWindowContents(Rect inRect) => settings.Build(inRect);

        internal static void Init()
        {
            TrackManager.Init();
            SoundManager.Init();
        }
    }
}
