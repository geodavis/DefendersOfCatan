using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.DAL.DataModels.Items
{
    public class Item
    {
        public int ItemType;
        public string ItemName;
        public List<ResourceCost> ItemCost;
    }
}