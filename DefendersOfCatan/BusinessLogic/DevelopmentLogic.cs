using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic
{
    public interface IDevelopmentLogic
    {
        List<DevelopmentTransfer> GetDevelopments();
        DevelopmentType PlaceInitialSettlement(int tileId);
        int PlacePurchasedRoad(int tile1Id, int tile2Id);
        void PlacePurchasedDevelopment(int parentTileId);
        List<List<int>> GetRoadPaths();
    }
    public class DevelopmentLogic : IDevelopmentLogic
    {
        private readonly IDevelopmentRepository _developmentRepo;
        private readonly ITileRepository _tileRepo;
        private readonly IPlayerRepository _playerRepo;
        private readonly ITileLogic _tileLogic;
        public DevelopmentLogic(IDevelopmentRepository developmentRepo, ITileRepository tileRepo, IPlayerRepository playerRepo, ITileLogic tileLogic)
        {
            _developmentRepo = developmentRepo;
            _tileRepo = tileRepo;
            _playerRepo = playerRepo;
            _tileLogic = tileLogic;

        }

        public List<DevelopmentTransfer> GetDevelopments()
        {
            var developmentsTransfer = new List<DevelopmentTransfer>();
            var developments = _developmentRepo.GetDevelopments();

            foreach (var development in developments)
            {
                var developmentTransfer = new DevelopmentTransfer
                {
                    DevelopmentType = development.DevelopmentType,
                    DevelopmentCost = development.DevelopmentCost,
                    DevelopmentTypeReadable = development.DevelopmentType.ToString()
                };
                developmentsTransfer.Add(developmentTransfer);
            }
            return developmentsTransfer;
        }

        public int PlacePurchasedRoad(int tile1Id, int tile2Id)
        {
            var angle = _tileRepo.PlaceRoad(tile1Id, tile2Id);           
            _playerRepo.RemoveDevelopmentFromCurrentPlayer(DevelopmentType.Road); // Remove development from player
            return angle;
        }


        public void PlacePurchasedDevelopment(int parentTileId)
        {
            var currentPlayerDevelopmentsWithQty = _developmentRepo.GetCurrentPlayerBase().PlayerDevelopments.Single(i => i.Qty > 0 && i.DevelopmentType != DevelopmentType.Card);
            var developmentType = currentPlayerDevelopmentsWithQty.DevelopmentType;
            if (currentPlayerDevelopmentsWithQty != null)
            {
                _tileRepo.AddDevelopmentToTile(parentTileId, developmentType);

            }

            // Remove development from player
            _playerRepo.RemoveDevelopmentFromCurrentPlayer(developmentType);
        }

        public List<List<int>> GetRoadPaths()
        {
            var paths = new List<List<int>>(); // Global paths list
            var tiles = _tileRepo.GetTiles().Where(t => t.Type == TileType.Resource || t.Type == TileType.Capital);
            foreach (var tile in tiles) // Loop each tile, traversing any roads on the tile.
            {
                if (!HasTileBeenTraversed(tile.Id, paths)) // If tile has already been traversed along a path, do not traverse again
                {
                    var tilePaths = new List<List<int>>(); // Tile paths list
                    TraverseRoads(tile, new List<int>(), new List<int>(), tilePaths);
                    foreach (var tilePath in tilePaths)
                    {
                        if (!DoesPathAlreadyExist(tilePath, paths))
                        {
                            paths.Add(tilePath);
                        }
                    }
                }
            }

            return paths;
        }
        public List<List<int>> TraverseRoads(Tile tile, List<int> traversedtileIds, List<int> path, List<List<int>> paths)
        {
            path.Add(tile.Id);

            var roads = _tileRepo.GetRoads().Where(r => r.Placed);
            var neighbors = _tileLogic.GetNeighborTiles(tile);
            var movableNeighbors = false;
            foreach (var neighborTile in neighbors)
            {
                if ((roads.Any(r => r.Tile1.Id == tile.Id && r.Tile2.Id == neighborTile.Id) ||
                roads.Any(r => r.Tile2.Id == tile.Id && r.Tile1.Id == neighborTile.Id)) &&
                !traversedtileIds.Contains(neighborTile.Id))
                {
                    movableNeighbors = true;
                    traversedtileIds.Add(tile.Id);
                    TraverseRoads(neighborTile, traversedtileIds, path, paths);
                }
            }

            if (!movableNeighbors)
            {
                if (path.Count() > 4)
                {            
                    paths.Add(new List<int>(path));
                }
            }
            path.RemoveAt(path.Count - 1);
            
            return paths;
        }

        private bool HasTileBeenTraversed(int tileId, List<List<int>> paths)
        {
            foreach (var tempPath in paths)
            {
                if (tempPath.Contains(tileId))
                {
                    return true;
                }
            }

            return false;
        }
        private bool DoesPathAlreadyExist(List<int> newPath, List<List<int>> existingPaths)
        {
            foreach (var existingPath in existingPaths)
            {
                if (newPath.All(existingPath.Contains))
                {
                    return true;
                }
            }

            return false;
        }
        public DevelopmentType PlaceInitialSettlement(int tileId)
        {
            var developmentType = DevelopmentType.Settlement;
            _tileRepo.AddDevelopmentToTile(tileId, developmentType);
            return developmentType;
        }

        public void RemoveSettlement(int tileId)
        {
            var developmentType = DevelopmentType.Settlement;
            _tileRepo.RemoveDevelopmentFromTile(tileId, developmentType);
        }
    }
}