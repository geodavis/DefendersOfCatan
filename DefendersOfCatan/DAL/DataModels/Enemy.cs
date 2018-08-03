using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class Enemy
    {
        public int Id { get; set; }
        public bool HasBarbarian { get; set; }
        public PlayerColor PlayerColor { get; set; }
        public bool HasBeenPlaced { get; set; }
        public int BarbarianIndex { get; set; } = 0;
        public bool IsRemoved { get; set; }
    }
}