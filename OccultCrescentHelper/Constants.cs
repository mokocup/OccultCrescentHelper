using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper
{
    internal static class Constants
    {
        public static readonly int OccultCrescentTerritoryId = 1252;
        public static readonly int OccultCrescentSouthHornMapId = 967;
        public static readonly int OccultCrescentBunnyFateId = 1976;
        public static readonly int OccultCrescentAuroralMirageWeatherId = 192;

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
                MapLink = Common.CreateSouthHornLink(position.X, position.Y);
            }
        }

        public static List<OccultCrescentFate> OccultCrescentFates =
        [
            new(1962, new Vector2(162, 676)),      // Rough Waters
            new(1963, new Vector2(373.2f, 486)),   // The Golden Guardian
            new(1964, new Vector2(-226.1f, 254)),  // King of the Crescent
            new(1965, new Vector2(-548.5f, -595)), // The Winged Terror
            new(1966, new Vector2(-223.1f, 36)),   // An Unending Duty
            new(1967, new Vector2(-48.1f, -320)),  // Brain Drain
            new(1968, new Vector2(-370, 650)),     // A Delicate Balance
            new(1969, new Vector2(-589.1f, 333)),  // Sworn to Soil
            new(1970, new Vector2(-71, 557)),      // A Prying Eye
            new(1971, new Vector2(79, 278)),       // Fatal Allure
            new(1972, new Vector2(413, -13)),      // Serving Darkness
            // new(1977, new Vector2(200, -215)), 
            // new(1977, new Vector2(-481, 528)),
        ];

        public record OccultCrescentCE
        {
            public readonly Vector2 Position;
            public readonly uint EventId;
            public readonly String Name;
            public readonly SeString MapLink;
            public long lastStartTime = 0;

            public OccultCrescentCE(uint eventId, Vector2 position)
            {
                var fateSheet = OccultCrescentHelper.DataManager.GameData.GetExcelSheet<DynamicEvent>().GetRow(eventId);
                EventId = eventId;
                Position = position;
                Name = fateSheet.Name.ToString();
                MapLink = Common.CreateSouthHornLink(position.X, position.Y);
            }
        }

        public static List<OccultCrescentCE> OccultCrescentCEs =
        [
            new(33, new Vector2(300, 730)),    // Scourge of the Mind
            new(34, new Vector2(450, 357)),    // The Black Regiment
            new(35, new Vector2(620, 800)),    // The Unbridled
            new(36, new Vector2(681, 534)),    // Crawling Death
            new(37, new Vector2(-340, 800)),   // Calamity Bound
            new(38, new Vector2(-414, 75)),    // Trial by Claw
            new(39, new Vector2(-800, 245)),   // From Times Bygone
            new(40, new Vector2(680, -256)),   // Company of Stone
            new(41, new Vector2(-117, -850)),  // Shark Attack
            new(42, new Vector2(636, -54)),    // On the Hunt
            new(43, new Vector2(-352, -608)),  // With Extreme Prejudice
            new(44, new Vector2(461, -363)),   // Noise Complaint
            new(45, new Vector2(72, -545)),    // Cursed Concern
            new(46, new Vector2(870.1f, 180)), // Eternal Watch
            new(47, new Vector2(-570, -160)),  // Flame of Dusk
        ];

        public static OccultCrescentCE ForkedTowerBloodCE = new(48, new Vector2(63, 4));
    }
}
