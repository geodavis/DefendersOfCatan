using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class PlayerItem
    {
        public int Id { get; set; }
        public ItemType ItemType { get; set; }
        public int Qty { get; set; }
    }
}