using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.Transfer
{
    public class EnemyMoveTransfer
    {
        public List<Tile> BarbarianTiles { get; set; }
        public List<Tile> OverrunTiles { get; set; }
        public List<Development> OverrunDevelopments { get; set; }
    }
}