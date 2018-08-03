using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class PlayerResourceTransfer
    {
        public int PlayerId { get; set; }
        public int ResourceType { get; set; }
    }
}