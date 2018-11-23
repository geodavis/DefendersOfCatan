using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.DAL;
using DefendersOfCatan.BusinessLogic.Repository;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic
{
    public class PlayerLogic
    {
        private readonly PlayerRepository playerRepo = new PlayerRepository();
        private readonly TileRepository tileRepo = new TileRepository();
        private readonly DevelopmentRepository developmentRepo = new DevelopmentRepository();
        private readonly TileLogic tileLogic = new TileLogic();

        public List<Player> GetPlayers() => playerRepo.GetPlayers();

        public Player GetCurrentPlayer() => playerRepo.GetCurrentPlayer();

        public bool MovePlayerToTile(int selectedTileId)
        {
            var currentPlayerTile = tileRepo.GetCurrentPlayerTile();
            var selectedTile = tileRepo.GetTileById(selectedTileId);
            var neighborTiles = tileLogic.GetNeighborTiles(currentPlayerTile);

            if (neighborTiles.Any(t => t.Id == selectedTile.Id) || selectedTile.Id == currentPlayerTile.Id)
            {
                tileRepo.UpdateCurrentPlayerTile(selectedTile);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddResourceToPlayer(ResourceType resourceType)
        {

            playerRepo.AddResourceToCurrentPlayer(resourceType);
        }

        public bool CheckIfPlayerIsOverrun(Player player)
        {
            var isOverrun = false;
            var tiles = tileRepo.GetTiles().Where(t => (int)t.Type == (int)player.Color).ToList();

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
            var developmentCost = developmentRepo.GetDevelopmentByType(developmentType).DevelopmentCost;
            var currentPlayer = GetCurrentPlayer();
            var playerResources = currentPlayer.PlayerResources;

            if (PlayerCanPurchaseDevelopment(developmentCost, playerResources))
            {
                // Take resources
                foreach (var cost in developmentCost)
                {
                    playerRepo.RemoveResourceFromCurrentPlayer((ResourceType)cost.ResourceType, cost.Qty);
                }

                // Save development to player
                playerRepo.AddDevelopmentToCurrentPlayer(developmentType);

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