using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.DAL;
using DefendersOfCatan.BusinessLogic.Repository;
using static DefendersOfCatan.Common.Enums;
using DefendersOfCatan.Transfer;

namespace DefendersOfCatan.BusinessLogic
{
    public interface IPlayerLogic
    {
        bool CheckIfPlayerIsOverrun(Player player);
        Player GetCurrentPlayer();
        bool MovePlayerToTile(int selectedTileId);
        void AddResourceToPlayer(ResourceType resourceType);
        bool PurchaseDevelopment(DevelopmentType developmentType);

    }
    public class PlayerLogic: IPlayerLogic
    {
        private readonly IPlayerRepository _playerRepo;
        private readonly ITileRepository _tileRepo;
        private readonly IDevelopmentRepository _developmentRepo;
        private readonly ITileLogic _tileLogic;

        public PlayerLogic(IDevelopmentRepository developmentRepo, ITileRepository tileRepo, IPlayerRepository playerRepo, ITileLogic tileLogic)
        {
            _developmentRepo = developmentRepo;
            _tileRepo = tileRepo;
            _playerRepo = playerRepo;
            _tileLogic = tileLogic;

        }

        public List<Player> GetPlayers() => _playerRepo.GetPlayers();

        public Player GetCurrentPlayer() => _playerRepo.GetCurrentPlayer();

        public bool MovePlayerToTile(int selectedTileId)
        {
            var currentPlayerTile = _tileRepo.GetCurrentPlayerTile();
            var selectedTile = _tileRepo.GetTileById(selectedTileId);
            var neighborTiles = _tileLogic.GetNeighborTiles(currentPlayerTile);

            if (neighborTiles.Any(t => t.Id == selectedTile.Id) || selectedTile.Id == currentPlayerTile.Id)
            {
                _tileRepo.UpdateCurrentPlayerTile(selectedTile);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddResourceToPlayer(ResourceType resourceType)
        {

            _playerRepo.AddResourceToCurrentPlayer(resourceType);
        }

        public bool CheckIfPlayerIsOverrun(Player player)
        {
            var isOverrun = false;
            var tiles = _tileRepo.GetTiles().Where(t => (int)t.Type == (int)player.Color).ToList();

            var count = 0;
            foreach (var tile in tiles)
            {
                if (tile.Enemy != null)
                {
                    count += 1;
                }
            }

            if (count == 3)
            {
                isOverrun = true;
            }

            return isOverrun;
        }

        public bool PurchaseDevelopment(DevelopmentType developmentType)
        {
            var developmentCost = _developmentRepo.GetDevelopmentByType(developmentType).DevelopmentCost;
            var playerResources = GetCurrentPlayer().PlayerResources;

            if (PlayerCanPurchaseDevelopment(developmentCost, playerResources))
            {
                // Take resources
                foreach (var cost in developmentCost)
                {
                    _playerRepo.RemoveResourceFromCurrentPlayer((ResourceType)cost.ResourceType, cost.Qty);
                }

                // Save development to player
                _playerRepo.AddDevelopmentToCurrentPlayer(developmentType);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool PlayerCanPurchaseDevelopment(List<ResourceCost> developmentCost, List<PlayerResource> playerResources)
        {
            var playerCanPurchase = false;

            foreach (var cost in developmentCost)
            {
                var requiredResourceType = cost.ResourceType;
                var requiredQty = cost.Qty;

                foreach (var resource in playerResources)
                {
                    var playerOwnedResourceType = resource.ResourceType;
                    var playerOwnedResourceQty = resource.Qty;

                    if (requiredResourceType == (int)playerOwnedResourceType)
                    {
                        if (playerOwnedResourceQty >= requiredQty)
                        {
                            playerCanPurchase = true;
                        }
                        else
                        {
                            playerCanPurchase = false;
                        }
                    }
                }
            }

            return playerCanPurchase;
        }

    }
}