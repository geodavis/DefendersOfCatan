using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.Transfer
{
    public class UpdateEnemyTransfer
    {
        public int Id { get; set; }
        public bool HasBeenPlaced { get; set; }
        public string CurrentHexName { get; set; }
        public int BarbarianIndex { get; set; }
    }
}