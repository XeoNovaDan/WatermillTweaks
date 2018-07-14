using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using Verse;
using RimWorld;

namespace WatermillTweaks
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {

        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            HarmonyInstance h = HarmonyInstance.Create("XeoNovaDan.BetterWatermillGenerators");

            h.Patch(AccessTools.Property(typeof(CompPowerPlantWater), "DesiredPowerOutput").GetGetMethod(true), null,
                new HarmonyMethod(patchType, nameof(PostfixDesiredPowerOutput)));

            h.Patch(AccessTools.Method(typeof(CompPowerPlantWater), nameof(CompPowerPlantWater.CompInspectStringExtra)), null,
                new HarmonyMethod(patchType, nameof(PostfixCompInspectStringExtra)));
        }

        public static void PostfixDesiredPowerOutput(CompPowerPlantWater __instance, ref float __result)
        {
            if (WatermillTweaksSettings.temperatureAffectsWatermillPowerGen)
                __result *= PowerOutputFactorFromTemperatureCurve.Evaluate(__instance.parent.MapHeld.mapTemperature.OutdoorTemp);
            if (__instance.parent.MapHeld.GameConditionManager.ConditionIsActive(BWG_GameConditionDefOf.TurbulentWaters))
                __result *= GameCondition_TurbulentWaters.WatermillPowerGenFactor;
        }

        public static void PostfixCompInspectStringExtra(CompPowerPlantWater __instance, ref string __result)
        {
            float powerProductionFactor = PowerOutputFactorFromTemperatureCurve.Evaluate(__instance.parent.MapHeld.mapTemperature.OutdoorTemp);
            if (WatermillTweaksSettings.temperatureAffectsWatermillPowerGen && powerProductionFactor != 1f)
                __result = __result + "\n" + "BadTemperature".Translate().CapitalizeFirst() + ": x" + powerProductionFactor.ToStringPercent();
        }

        private static SimpleCurve PowerOutputFactorFromTemperatureCurve = new SimpleCurve
        {
            { new CurvePoint(WatermillTweaksSettings.watermillPlantNoProductionBelow, 0f), true },
            { new CurvePoint(WatermillTweaksSettings.watermillPlantHalfProductionAtLow, 0.5f), true },
            { new CurvePoint(WatermillTweaksSettings.watermillPlantMinOptimalTemp, 1f), true },
            { new CurvePoint(WatermillTweaksSettings.watermillPlantMaxOptimalTemp, 1f), true },
            { new CurvePoint(WatermillTweaksSettings.watermillPlantHalfProductionAtHigh, 0.5f), true },
            { new CurvePoint(WatermillTweaksSettings.watermillPlantNoProductionAbove, 0f), true },
        };

    }
}
