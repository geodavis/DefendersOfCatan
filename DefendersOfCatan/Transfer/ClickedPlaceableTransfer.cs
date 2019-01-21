using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class ClickedPlaceableTransfer
    {
        public int ParentTileId { get; set; }
        public DevelopmentType developmentType { get; set; }

    }
}