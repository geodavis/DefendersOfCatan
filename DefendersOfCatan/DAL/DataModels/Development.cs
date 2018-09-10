using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class Development
    {
        public int Id { get; set; }
        public DevelopmentType DevelopmentType { get; set; }
        public string DevelopmentName { get; set; }
        public virtual List<ResourceCost> DevelopmentCost { get; set; }
    }
}