using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace WatermillTweaks
{
    public class IncidentWorker_TurbulentWaters : IncidentWorker_MakeGameCondition
    {

        public override float AdjustedChance
        {
            get
            {
                float finalCommonality = 0f;
                try
                {
                    if (map != null)
                    {
                        RiverDef river = map.TileInfo.Rivers[(map.TileInfo.Rivers.Count - 1)].river;
                        if (river == RiverDefOf.River)
                            finalCommonality = def.baseChance;
                        else if (river == RiverDefOf.LargeRiver)
                            finalCommonality = def.baseChance * 2f;
                        else if (river == RiverDefOf.HugeRiver)
                            finalCommonality = def.baseChance * 4f;
                        finalCommonality *= RainfallToAdjustedChanceFactorCurve.Evaluate(map.TileInfo.rainfall);
                    }
                }
                catch (NullReferenceException)
                {
                    finalCommonality = def.baseChance;
                }
                return finalCommonality;
            }
        }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map currentMap = (Map)parms.target;
            map = currentMap;
            return !currentMap.TileInfo.Rivers.NullOrEmpty() && currentMap.mapTemperature.SeasonalTemp >= 15f;
        }

        private static readonly SimpleCurve RainfallToAdjustedChanceFactorCurve = new SimpleCurve
        {
            { new CurvePoint(0f, 0f) },
            { new CurvePoint(200f, 0.5f) },
            { new CurvePoint(1000f, 1f) },
            { new CurvePoint(2000f, 1f) },
            { new CurvePoint(3000f, 1.3f) },
            { new CurvePoint(5000f, 2f) },
        };

        private Map map;

    }
}
