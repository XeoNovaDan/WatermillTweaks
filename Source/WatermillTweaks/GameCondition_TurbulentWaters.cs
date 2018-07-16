using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace WatermillTweaks
{
    public class GameCondition_TurbulentWaters : GameCondition
    {

        public const float WatermillPowerGenFactor = 1.5f;

        private const int WatermillListUpdateInterval = 600;

        private List<Building> watermillGeneratorsToAffect = new List<Building>();

        private int nextWatermillDamageTicks = 0;

        private static readonly IntRange BaseTicksBetweenWatermillDamage = new IntRange (1200, 3600);

        private static readonly SimpleCurve RandomDamageAmountCurve = new SimpleCurve
        {
            { new CurvePoint(0f, 5f), true },
            { new CurvePoint(0.1f, 5f), true },
            { new CurvePoint(0.73f, 15f), true },
            { new CurvePoint(0.83f, 15f), true },
            { new CurvePoint(0.831f, 25f), true },
            { new CurvePoint(0.88f, 25f), true },
            { new CurvePoint(0.98f, 40f), true },
            { new CurvePoint(1f, 40f), true },
        };

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextWatermillDamageTicks, "nextWatermillDamageTicks", 0);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                UpdateWaterGeneratorsToAffect(true);
            }
        }

        public override void Init()
        {
            base.Init();
            UpdateWatermillDamageTicks();
            UpdateWaterGeneratorsToAffect();
        }

        private void UpdateWaterGeneratorsToAffect(bool forceUpdate = false)
        {
            if (Find.TickManager.TicksGame % WatermillListUpdateInterval == 0 || forceUpdate)
            {
                watermillGeneratorsToAffect.Clear();
                for (int i = 0; i < AffectedMaps.Count; i++)
                {
                    Map map = AffectedMaps[i];
                    for (int j = 0; j < map.listerBuildings.allBuildingsColonist.Count; j++)
                    {
                        Building currentBuilding = map.listerBuildings.allBuildingsColonist[j];
                        if (currentBuilding.def == ThingDefOf.WatermillGenerator)
                        {
                            watermillGeneratorsToAffect.Add(currentBuilding);
                        }
                    }
                }
            }
        }

        private void UpdateWatermillDamageTicks()
        {
            nextWatermillDamageTicks = Find.TickManager.TicksGame + BaseTicksBetweenWatermillDamage.RandomInRange / Math.Max(watermillGeneratorsToAffect.Count(), 1);
        }

        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame > nextWatermillDamageTicks && watermillGeneratorsToAffect.Count() != 0)
            {
                IntRange generatorToSelect = new IntRange(0, watermillGeneratorsToAffect.Count() - 1);
                Thing generatorToDamage = watermillGeneratorsToAffect[generatorToSelect.RandomInRange];
                generatorToDamage.TakeDamage(new DamageInfo(DamageDefOf.Blunt, Mathf.RoundToInt(RandomDamageAmountCurve.Evaluate(Rand.Value))));
                UpdateWatermillDamageTicks();
            }
            UpdateWaterGeneratorsToAffect();
        }

    }
}
