using Lumina.Excel.Sheets;

namespace OccultCrescentHelper
{
    internal static class Common
    {
        public static bool IsPlayerInSouthHorn(int territoryId, uint mapId)
        {
            return territoryId == Constants.OccultCrescentTerritoryId &&
                   mapId == Constants.OccultCrescentSouthHornMapId;
        }

        public static float ToMapCoordinate(float val, uint mapId)
        {
            var scale = OccultCrescentHelper.DataManager.GameData.GetExcelSheet<Map>().GetRow(mapId).SizeFactor;
            var c = scale / 100f;

            val *= c;
            return (41f / c * ((val + 1024f) / 2048f)) + 1;
        }

        public static float ToMapCoordinate(float val, float scale = 1)
        {
            var c = scale / 100f;

            val *= c;
            return (41f / c * ((val + 1024f) / 2048f)) + 1;
        }
    }
}
