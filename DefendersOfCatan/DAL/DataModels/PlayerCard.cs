using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class PlayerCard
    {
        public int Id { get; set; }
        public CardType CardType { get; set; }
        public int Qty { get; set; }
    }
}