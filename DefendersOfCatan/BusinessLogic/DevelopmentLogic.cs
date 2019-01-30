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
        void GetRoadPaths();
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

        public void GetRoadPaths()
        {
            var roadPaths = new List<List<int>>();
            var roads = _tileRepo.GetRoads().Where(r => r.Placed);
            Debug.WriteLine("New Road Placed ............");
            foreach (var road in roads)
            {
                var tile1 = _tileRepo.GetTiles().Where(t => t.Id == road.Tile1.Id).Single();// ToDo: Implement Tile2.Id to pick up other direction
                var traversedTile1Ids = new List<int>();
                //traversedTile1Ids.Add(tile1.Id);
                Debug.WriteLine("New Path Road 1...");
                var paths = new List<List<int>>();
                var path = new List<int>();
                var path1 = TraverseRoads(tile1.Id, tile1, traversedTile1Ids, path, paths);
            }
        }
        public List<List<int>> TraverseRoads(int startTileId, Tile tile, List<int> traversedtileIds, List<int> path, List<List<int>> paths)
        {
            Debug.WriteLine("Moved to tile: " + tile.Id);
            path.Add(tile.Id);

            var roads = _tileRepo.GetRoads().Where(r => r.Placed);
            var neighbors = _tileLogic.GetNeighborTiles(tile);
            var movableNeighbors = false;
            foreach (var neighborTile in neighbors)
            {
                if ((roads.Any(r => r.Tile1.Id == tile.Id && r.Tile2.Id == neighborTile.Id) || // Check if neighbor has been traversed
                roads.Any(r => r.Tile2.Id == tile.Id && r.Tile1.Id == neighborTile.Id)) &&
                !traversedtileIds.Contains(neighborTile.Id))
                {
                    movableNeighbors = true;
                    if (traversedtileIds.Contains(tile.Id)) // Check if backtrack. If so, we are on a new path.
                    {
                        Debug.WriteLine("New path needed...");
                    //    //paths.Add(new List<int>(path));
                    //    //path.RemoveAt(path.Count - 1);
                    }
                    traversedtileIds.Add(tile.Id);
                    TraverseRoads(startTileId, neighborTile, traversedtileIds, path, paths);
                }
            }

            if (!movableNeighbors)
            {
                paths.Add(new List<int>(path));
            }
            path.RemoveAt(path.Count - 1);


            //if (traversedtileIds.Contains(tile.Id))
            //{
            //    paths.Add(new List<int>(path));
            //}

            return paths;
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