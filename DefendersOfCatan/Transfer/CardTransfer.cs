using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Transfer
{
    public class CardTransfer
    {
        public CardType CardType { get; set; }
        public string CardTypeReadable { get; set; }
        public string CardDescription { get; set; }
    }
}