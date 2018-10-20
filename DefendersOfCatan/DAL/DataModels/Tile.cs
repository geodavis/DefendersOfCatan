using System.Collections.Generic;
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
        public virtual List<Player> Players { get; set; } = new List<Player>();
        public bool IsOverrun {get; set; }
        public bool IsSelected { get; set; }
        public virtual List<TileDevelopment> Developments { get; set; }

        
        public bool IsEnemyTile()
        {
            return (Type == TileType.BlueEnemy) || (Type == TileType.GreenEnemy) || (Type == TileType.RedEnemy) || (Type == TileType.YellowEnemy);

        }
    }

}