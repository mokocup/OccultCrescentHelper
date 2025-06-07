using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OccultCrescentHelper
{
    internal static class Constants
    {
        public static int OccultCrescentTerritoryId = 1252;
        public static int OccultCrescentSouthHornMapId = 967;
        public static int OccultCrescentBunnyFateId = 1976;
        public static int OccultCrescentAuroralMirageWeatherId = 192;

        public static Vector2 OccultCrescentSouthHornForkedTowerEntryPosition = new Vector2(64, 4);


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
    }
}
