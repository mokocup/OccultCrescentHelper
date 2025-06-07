using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OccultCrescentHelper
{
    internal static class Common
    {
        public static bool IsPlayerInSouthHorn(int territoryId, uint mapId)
        {
            return territoryId == Constants.OccultCrescentTerritoryId &&
                   mapId == Constants.OccultCrescentSouthHornMapId;
        }
    }
}
