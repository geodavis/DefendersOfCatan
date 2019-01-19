using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class PurchaseDevelopmentTransfer
    {
        public DevelopmentType DevelopmentType { get; set; }
        public List<Tile> Tiles { get; set; }
        public bool CanPurchase { get; set; }
        public List<Road> Roads { get; set; }
    }
}