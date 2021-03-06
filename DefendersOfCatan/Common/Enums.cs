﻿using System;
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
        public enum DevelopmentType
        {
            Road,
            Settlement,
            City,
            Walls,
            Card,
        }

        public enum CardType
        {
            BarbarianBack, // Move one set of player enemy tile barbarians back one space
            EnemyRemove, // Remove any one enemy of your choosing
            PlayerMove, // Move to any tile of your choosing
            FreeDevelopment // Purchase any development of your choosing
        }
        public enum GameState
        {
            Initialization,
            InitialPlacement,
            EnemyMove,
            EnemyOverrun,
            EnemyCard,
            PlayerPurchase,
            //PlayerPlacePurchase,
            PlayerMove,
            PlayerResourceOrFight
        }

    }
}