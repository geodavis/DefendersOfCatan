using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.Transfer
{
    public class ClickedEnemyTransfer
    {
        public int EnemyId { get; set; }
        public string GameState { get; set; }
        public int EnemyTileId { get; set; }
    }
}