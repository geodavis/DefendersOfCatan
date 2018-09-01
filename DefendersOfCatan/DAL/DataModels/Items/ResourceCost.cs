using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.DAL.DataModels.Items
{
    public class ResourceCost
    {
        public int Id { get; set; }
        public int ResourceType { get; set; }
        public int Qty { get; set; }
    }
}