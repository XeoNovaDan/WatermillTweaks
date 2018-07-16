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

        private const float PowerProductionFactorSpring = 1.3f;
        private const float PowerProductionFactorSummer = 1f;
        private const float PowerProductionFactorFall = 0.9f;
        private const float PowerProductionFactorWinter = 0.8f;

        private const float ZeroPowerProductionLowTemp = -40f;
        private const float HalfPowerProductionLowTemp = -10f;
        private const float FullPowerProductionLowTemp = 0f;
        private const float FullPowerProductionHighTemp = 50f;
        private const float HalfPowerProductionHighTemp = 60f;
        private const float ZeroPowerProductionHighTemp = 90f;

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
            // Season
            __result *= SeasonalPowerOutputFactor(GetWatermillMapSeason(__instance.parent));

            // Outdoor Temperature
            __result *= PowerOutputFactorFromTemperatureCurve.Evaluate(__instance.parent.MapHeld.mapTemperature.OutdoorTemp);

            // Turbulent Waters
            if (__instance.parent.MapHeld.GameConditionManager.ConditionIsActive(BWG_GameConditionDefOf.TurbulentWaters))
                __result *= GameCondition_TurbulentWaters.WatermillPowerGenFactor;
        }

        public static void PostfixCompInspectStringExtra(CompPowerPlantWater __instance, ref string __result)
        {
            // Season
            Season season = GetWatermillMapSeason(__instance.parent);
            float seasonalPowerProductionFactor = SeasonalPowerOutputFactor(season);
            __result += "\n" + season.LabelCap() + ": x" + seasonalPowerProductionFactor.ToStringPercent();

            // Outdoor Temperature
            float tempPowerProductionFactor = PowerOutputFactorFromTemperatureCurve.Evaluate(__instance.parent.MapHeld.mapTemperature.OutdoorTemp);
            if (tempPowerProductionFactor != 1f)
                __result += "\n" + "BadTemperature".Translate().CapitalizeFirst() + ": x" + tempPowerProductionFactor.ToStringPercent();
        }

        private static Season GetWatermillMapSeason(Thing powerPlant)
        {
            return GenDate.Season(Find.TickManager.TicksAbs, Find.WorldGrid.LongLatOf(powerPlant.MapHeld.Tile));
        }

        private static float SeasonalPowerOutputFactor(Season season)
        {
            switch (season)
            {
                case Season.PermanentWinter:
                    return PowerProductionFactorWinter;
                case Season.Winter:
                    return PowerProductionFactorWinter;
                case Season.Fall:
                    return PowerProductionFactorFall;
                case Season.Spring:
                    return PowerProductionFactorSpring;
                default:
                    return PowerProductionFactorSummer;
            }
        }

        private static SimpleCurve PowerOutputFactorFromTemperatureCurve = new SimpleCurve
        {
            { new CurvePoint(ZeroPowerProductionLowTemp, 0f), true },
            { new CurvePoint(HalfPowerProductionLowTemp, 0.5f), true },
            { new CurvePoint(FullPowerProductionLowTemp, 1f), true },
            { new CurvePoint(FullPowerProductionHighTemp, 1f), true },
            { new CurvePoint(HalfPowerProductionHighTemp, 0.5f), true },
            { new CurvePoint(ZeroPowerProductionHighTemp, 0f), true },
        };

    }
}
