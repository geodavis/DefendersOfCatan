using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class PlayerResource
    {
        public int Id { get; set; }
        public ResourceType ResourceType { get; set; }
        public int Qty { get; set; }
        //public Player Player { get; set; }
    }
}