using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.Transfer
{
    public class EnemyFightTransfer
    {
        public Tile EnemyTile { get; set; }
        public List<int> DiceRolls { get; set; } = new List<int>();
        public bool EnemyHit { get; set; } = false;
        public bool CanReach { get; set; } = false;
        public string Message { get; set; }
    }
}