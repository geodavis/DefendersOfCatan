using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.Common
{
    public static class Enums
    {
        public enum TileType
        {
            RedEnemy,
            BlueEnemy,
            YellowEnemy,
            GreenEnemy,
            Resource,
            Capital,
            Unused,
            Hidden
        }
        public enum PlayerColor
        {
            Red,
            Blue,
            Yellow,
            Green
        }
        public enum ResourceType
        {
            Brick,
            Ore,
            Wood,
            Grain,
            Wool,
            NoResource
        }
        public enum ItemType
        {
            Item1,
            Item2,
        }
        public enum GameState
        {
            Initialization,
            EnemyMove,
            EnemyOverrun,
            EnemyCard,
            PlayerMove,
            PlayerResourceOrFight
        }

    }
}