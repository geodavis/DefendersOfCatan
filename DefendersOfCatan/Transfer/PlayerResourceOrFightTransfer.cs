using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.Transfer
{
    public class PlayerResourceOrFightTransfer
    {
        public List<int> ResourceTiles { get; set; }
        public List<int> FightTiles { get; set; }
    }
}