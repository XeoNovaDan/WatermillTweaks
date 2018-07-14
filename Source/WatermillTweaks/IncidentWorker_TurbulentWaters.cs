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
                try
                {
                    if (map != null)
                    {
                        RiverDef river = map.TileInfo.Rivers[(map.TileInfo.Rivers.Count - 1)].river;
                        if (river == RiverDefOf.River)
                            return def.baseChance;
                        else if (river == RiverDefOf.LargeRiver)
                            return def.baseChance * 2f;
                        else if (river == RiverDefOf.HugeRiver)
                            return def.baseChance * 4f;
                    }
                    return 0f;
                }
                catch (NullReferenceException)
                {
                    return def.baseChance;
                }
            }
        }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map currentMap = (Map)parms.target;
            map = currentMap;
            return !currentMap.TileInfo.Rivers.NullOrEmpty() && currentMap.mapTemperature.SeasonalTemp >= 0f;
        }

        private Map map;

    }
}
