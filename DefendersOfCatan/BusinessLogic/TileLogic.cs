using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.Common;
using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic
{
    public class TileLogic
    {
        private TileRepository tileRepo = new TileRepository();
        public Tile GetCurrentPlayerTile()
        {
            return tileRepo.GetCurrentPlayerTile();
        }

        public List<Tile> GetNeighborTiles(Tile tile)
        {
            var neighboringTiles = new List<Tile>();
            //            element[x, y]
            //            neighbor1 = x + 1, y;
            //            neighbor2 = x - 1, y;
            //            neighbor3 = x, y + 1;
            //            neighbor4 = x, y - 1;
            //            neighbor5 = x + 1, y + 1; - NOT A HEX NEIGHBOR FOR EVEN ROW
            //            neighbor6 = x + 1, y - 1; - NOT A HEX NEIGHBOR FOR EVEN ROW
            //            neighbor7 = x - 1, y + 1; - NOT A HEX NEIGHBOR FOR ODD ROW
            //            neighbor8 = x - 1, y - 1; - NOT A HEX NEIGHBOR FOR ODD ROW

            // Each hex has 6 neighbors - the below 4 are always a neighbor, regardless if even or odd row
            AddNeighbor(tile.LocationX + 1, tile.LocationY, neighboringTiles);
            AddNeighbor(tile.LocationX - 1, tile.LocationY, neighboringTiles);
            AddNeighbor(tile.LocationX, tile.LocationY + 1, neighboringTiles);
            AddNeighbor(tile.LocationX, tile.LocationY - 1, neighboringTiles);

            // The next two neighbors will change depending on if it is an even or odd row
            if (tile.LocationY % 2 != 0) // odd row
            {
                AddNeighbor(tile.LocationX + 1, tile.LocationY + 1, neighboringTiles);
                AddNeighbor(tile.LocationX + 1, tile.LocationY - 1, neighboringTiles);

            }
            else // even row
            {
                AddNeighbor(tile.LocationX - 1, tile.LocationY + 1, neighboringTiles);
                AddNeighbor(tile.LocationX - 1, tile.LocationY - 1, neighboringTiles);
            }

            return neighboringTiles;
        }
        
        private void AddNeighbor(int x, int y, List<Tile> neighboringTiles)
        {
            var tiles = tileRepo.GetTiles();
            if (tiles.Where(t => t.LocationX == x && t.LocationY == y).Any())
            {
                neighboringTiles.Add(tiles.Where(t => t.LocationX == x && t.LocationY == y).Single());
            }
        }

        public Tile SetOverrunTile(Tile tile)
        {
            var tileNumber = Globals.HexOverrunData[tile.LocationY, tile.LocationX];
            var overrunTile = GetNextOverrunTile(tile, tileNumber, 0);
            tileRepo.SetOverrunTile(overrunTile);
            return overrunTile;
        }

        public bool TileHasSettlement(int tileId)
        {
            var tileHasSettlement = false;
            var tileDevelopments = tileRepo.GetDevelopments(tileId);
            if (tileDevelopments.Any(t => t.Development.DevelopmentType == DevelopmentType.Settlement))
            {
                tileHasSettlement = true;
            }
            return tileHasSettlement;
        }

        private Tile GetNextOverrunTile(Tile tile, string tileNumber, int tileCount)
        {
            // Get next tile in line
            // First, get neighbor with tile number
            var neighbors = GetNeighborTiles(tile);

            foreach (var neighbor in neighbors)
            {
                var overrunData = Globals.HexOverrunData[neighbor.LocationY, neighbor.LocationX];
                var splitOverrunData = overrunData.Split(',');

                if (splitOverrunData.Contains(tileNumber) == true && !neighbor.IsEnemyTile()) // if is enemy tile, do not consider that neighbor tile
                {
                    if (!neighbor.IsOverrun)
                    {
                        return neighbor;
                    }
                    else // neighbor tile is overrun
                    {
                        tileCount += 1;
                        if (tileCount == 4 || neighbor.Type == TileType.Capital)
                        {
                            Console.WriteLine("game over!"); // todo: second blue tile (46) flipping ends game - BUG; need to check entire row for count, not just the direction we are coming; do need a check end game state function
                        }
                        return GetNextOverrunTile(neighbor, tileNumber, tileCount); // if tile is overrun, move onto the next tile in line
                    }
                }
            }

            Console.WriteLine("Error getting next overrun tile!");
            return new Tile(); // Error condition if it reaches here!!!
        }

    }
}