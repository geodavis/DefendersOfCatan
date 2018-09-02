using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels.Items
{
    public class Item
    {
        public int Id { get; set; }
        public ItemType ItemType { get; set; }
        public string ItemName { get; set; }
        public virtual List<ResourceCost> ItemCost { get; set; }
    }
}