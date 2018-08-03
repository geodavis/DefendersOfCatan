using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class BoardDataTransfer
    {
        public int[,] TileLayout { get; set; }
        public List<ResourceType> RandomResourceTypes { get; set; }

    }
}