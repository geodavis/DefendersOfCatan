using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class CardPlaceableTransfer
    {
        public CardType CardType { get; set; }
        public List<int> TileIds { get; set; }
    }
}