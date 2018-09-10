using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class DevelopmentTransfer
    {
        public DevelopmentType DevelopmentType { get; set; }
        public string DevelopmentTypeReadable { get; set; }
        public virtual List<ResourceCost> DevelopmentCost { get; set; }
    }
}