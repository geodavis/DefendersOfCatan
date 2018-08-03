using System.Collections.Generic;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PlayerColor Color { get; set; }
        public string CurrentHexName { get; set; }
        public bool IsOverrun { get; set; }
        public int Health { get; set; }
        public List<PlayerResource> PlayerResources { get; set; }
    }
}