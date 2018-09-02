using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.DAL;
using DefendersOfCatan.BusinessLogic.Repository;
using static DefendersOfCatan.Common.Enums;
using DefendersOfCatan.DAL.DataModels.Items;

namespace DefendersOfCatan.BusinessLogic
{
    public class PlayerLogic
    {
        private PlayerRepository playerRepo = new PlayerRepository();
        private TileRepository tileRepo = new TileRepository();
        private ItemRepository itemRepo = new ItemRepository();
        private TileLogic tileLogic = new TileLogic();

        public List<Player> GetPlayers()
        {
            return playerRepo.GetPlayers();
        }

        public Player GetCurrentPlayer()
        {
            return playerRepo.GetCurrentPlayer();
        }

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

        public bool PurchaseItem(ItemType itemType)
        {
            var itemCost = itemRepo.GetItemByType(itemType).ItemCost;
            var currentPlayer = GetCurrentPlayer();
            var playerResources = currentPlayer.PlayerResources;

            if (PlayerCanPurchaseItem(itemCost, playerResources))
            {
                // Take resources
                foreach (var cost in itemCost)
                {
                    playerRepo.RemoveResourceFromCurrentPlayer((ResourceType)cost.ResourceType, cost.Qty);
                }

                // Save item to player
                playerRepo.AddItemToCurrentPlayer(itemType);

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool PlayerCanPurchaseItem(List<ResourceCost> itemCost, List<PlayerResource> playerResources)
        {
            var playerCanPurchase = false;

            foreach (var cost in itemCost)
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