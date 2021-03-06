﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PlayerColor Color { get; set; }
        public bool IsOverrun { get; set; }
        public bool HasPurchasedItems { get; set; }
        public int Health { get; set; }
        public virtual List<PlayerResource> PlayerResources { get; set; }
        public virtual List<PlayerDevelopment> PlayerDevelopments { get; set; }
        public virtual List<PlayerCard> PlayerCards { get; set; }
        public bool CanMoveToAnyTile { get; set; }
        public bool CanPurchaseAnyDevelopment { get; set; }
    }
}