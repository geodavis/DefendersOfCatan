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
        public List<int> DiceRolls { get; set; } = new List<int>();
        public bool EnemyHit { get; set; } = false;
        public int OverrunPlayerId { get; set; }
    }
}