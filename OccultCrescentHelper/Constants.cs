using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper
{
    internal static class Constants
    {
        public static int OccultCrescentTerritoryId = 1252;
        public static int OccultCrescentSouthHornMapId = 967;
        public static int OccultCrescentBunnyFateId = 1976;
        public static int OccultCrescentAuroralMirageWeatherId = 192;

        public static Vector2 OccultCrescentSouthHornForkedTowerEntryPosition = new Vector2(63, 4);

        public record SoundSFX
        {
            public String Name;
            public uint EffectId;

            public SoundSFX(int name, uint effectId)
            {
                Name = $"<se.{name}>";
                EffectId = effectId;
            }
        }

        public static List<SoundSFX> PlayableSoundEffect =
        [
            new SoundSFX(1, 36), new SoundSFX(2, 37), new SoundSFX(3, 38), new SoundSFX(4, 39), new SoundSFX(5, 40),
            new SoundSFX(6, 41), new SoundSFX(7, 42), new SoundSFX(8, 43), new SoundSFX(9, 44), new SoundSFX(10, 45),
            new SoundSFX(11, 46), new SoundSFX(12, 47), new SoundSFX(13, 48), new SoundSFX(14, 49),
            new SoundSFX(15, 50), new SoundSFX(16, 51), new SoundSFX(17, 52)
        ];

        public record OccultCrescentFate
        {
            public readonly Vector2 Position;
            public readonly uint FateId;
            public readonly String Name;
            public readonly SeString MapLink;

            public OccultCrescentFate(uint fateId, Vector2 position)
            {
                var fateSheet = OccultCrescentHelper.DataManager.GameData.GetExcelSheet<Fate>().GetRow(fateId);
                FateId = fateId;
                Position = position;
                Name = fateSheet.Name.ToString();
                MapLink = SeString.CreateMapLink((ushort)OccultCrescentTerritoryId, (uint)OccultCrescentSouthHornMapId,
                                                 Common.ToMapCoordinate(Position.X, (uint)OccultCrescentSouthHornMapId),
                                                 Common.ToMapCoordinate(
                                                     Position.Y, (uint)OccultCrescentSouthHornMapId));
            }
        }

        public static List<OccultCrescentFate> OccultCrescentFates =
        [
            new(1962, new Vector2(162, 676)),
            new(1963, new Vector2(373.2f, 486)),
            new(1964, new Vector2(-226.1f, 254)),
            new(1965, new Vector2(-548.5f, -595)),
            new(1966, new Vector2(-223.1f, 36)),
            new(1967, new Vector2(-48.1f, -320)),
            new(1968, new Vector2(-370, 650)),
            new(1969, new Vector2(-589.1f, 333)),
            new(1970, new Vector2(-71, 557)),
            new(1971, new Vector2(79, 278)),
            new(1972, new Vector2(413, -13)),
            new(1977, new Vector2(200, -215)),
            new(1977, new Vector2(-481, 528)),
        ];
    }
}
