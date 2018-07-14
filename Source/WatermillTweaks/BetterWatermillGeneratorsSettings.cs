using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;

namespace WatermillTweaks
{
    public class WatermillTweaksSettings : ModSettings
    {

        public static bool temperatureAffectsWatermillPowerGen = true;
        public static int watermillPlantNoProductionBelow = -40;
        public static int watermillPlantHalfProductionAtLow = 0;
        public static int watermillPlantMinOptimalTemp = 10;
        public static int watermillPlantMaxOptimalTemp = 90;
        public static int watermillPlantHalfProductionAtHigh = 100;
        public static int watermillPlantNoProductionAbove = 150;

        public void DoWindowContents(Rect wrect)
        {
            Listing_Standard options = new Listing_Standard();
            Color defaultColor = GUI.color;
            options.Begin(wrect);

            GUI.color = defaultColor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            options.Gap();
            options.CheckboxLabeled("TempAffectsWatermillPowerGen".Translate(), ref temperatureAffectsWatermillPowerGen);
            options.Gap();
            if (!temperatureAffectsWatermillPowerGen) GUI.color = Color.grey;
            Text.Font = GameFont.Medium;
            options.Label("WatermillPlantTemperatureCustomization".Translate());
            Text.Font = GameFont.Small;
            options.Gap();
            options.AddLabeledNumericalTextField("WatermillPlantNoProductionBelow".Translate(), ref watermillPlantNoProductionBelow, 0.5f, -273, 2000);
            options.AddLabeledNumericalTextField("WatermillPlantHalfProduction".Translate(), ref watermillPlantHalfProductionAtLow, 0.5f, -273, 2000);
            options.AddLabeledNumericalTextField("WatermillPlantMinOptimalTemp".Translate(), ref watermillPlantMinOptimalTemp, 0.5f, -273, 2000);
            options.AddLabeledNumericalTextField("WatermillPlantMaxOptimalTemp".Translate(), ref watermillPlantMaxOptimalTemp, 0.5f, -273, 2000);
            options.AddLabeledNumericalTextField("WatermillPlantHalfProduction".Translate(), ref watermillPlantHalfProductionAtHigh, 0.5f, -273, 2000);
            options.AddLabeledNumericalTextField("WatermillPlantNoProductionAbove".Translate(), ref watermillPlantNoProductionAbove, 0.5f, -273, 2000);

            GUI.color = defaultColor;

            options.End();

            Mod.GetSettings<WatermillTweaksSettings>().Write();

        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref temperatureAffectsWatermillPowerGen, "temperatureAffectsWatermillPowerGen", false);
            Scribe_Values.Look(ref watermillPlantNoProductionBelow, "watermillPlantNoProductionBelow", -40);
            Scribe_Values.Look(ref watermillPlantHalfProductionAtLow, "watermillPlantHalfProductionAtLow", 0);
            Scribe_Values.Look(ref watermillPlantMinOptimalTemp, "watermillPlantMinOptimalTemp", 10);
            Scribe_Values.Look(ref watermillPlantMaxOptimalTemp, "watermillPlantMaxOptimalTemp", 90);
            Scribe_Values.Look(ref watermillPlantHalfProductionAtHigh, "watermillPlantHalfProductionAtHigh", 100);
            Scribe_Values.Look(ref watermillPlantNoProductionAbove, "watermillPlantNoProductionAbove", 150);
        }

    }

    public class WatermillTweaks : Mod
    {
        public WatermillTweaksSettings settings;

        public WatermillTweaks(ModContentPack content) : base(content)
        {
            GetSettings<WatermillTweaksSettings>();
        }

        public override string SettingsCategory() => "WatermillTweaksSettingsCategory".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            GetSettings<WatermillTweaksSettings>().DoWindowContents(inRect);
        }
    }
}