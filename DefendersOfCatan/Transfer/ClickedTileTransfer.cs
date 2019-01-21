using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class ClickedTileTransfer
    {
        public int ClickedTileId { get; set; }
        public int EnemyId { get; set; }
        public string GameState { get; set; }
        public int PlayerId { get; set; }
        public int ResourceType { get; set; }
        public int DevelopmentType { get; set; }
        public int Angle { get; set; }
        public int RoadTile1Id { get; set; }
        public int RoadTile2Id { get; set; }
        
        
    }
}