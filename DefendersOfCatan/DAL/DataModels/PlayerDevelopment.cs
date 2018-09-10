using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class PlayerDevelopment
    {
        public int Id { get; set; }
        public DevelopmentType DevelopmentType { get; set; }
        public int Qty { get; set; }
    }
}