using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class TileDevelopment
    {
        public int Id { get; set; }
        public virtual Development Development { get; set; }
    }
}