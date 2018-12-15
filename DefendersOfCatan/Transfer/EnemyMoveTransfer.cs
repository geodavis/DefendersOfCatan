using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class EnemyMoveTransfer
    {
        public List<Tile> BarbarianTiles { get; set; }
        public List<Tile> OverrunTiles { get; set; }
        public List<OverrunTileDevelopment> OverrunDevelopments { get; set; }
    }

    public class OverrunTileDevelopment
    {
        public DevelopmentType DevelopmentType { get; set; }
        public Tile Tile { get; set; }
    }
}