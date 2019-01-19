using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.Transfer
{
    public class ClickedPlaceableTransfer
    {
        public int ClickedPlaceableParentTileId { get; set; }
        public int Angle { get; set; }

    }
}