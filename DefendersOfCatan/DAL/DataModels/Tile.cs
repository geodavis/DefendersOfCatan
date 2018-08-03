using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class Tile
    {
        public int Id { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public TileType Type { get; set; }
        public PlayerColor PlayerColor { get; set; }
        public ResourceType ResourceType { get; set; }
        public virtual Enemy Enemy { get; set; }
        public string Name { get; set; }
        public Player Player { get; set; }
        public bool IsOverrun {get; set; }

        
        public bool IsEnemyTile()
        {
            return (Type == TileType.BlueEnemy) || (Type == TileType.GreenEnemy) || (Type == TileType.RedEnemy) || (Type == TileType.YellowEnemy);

        }
    }

}