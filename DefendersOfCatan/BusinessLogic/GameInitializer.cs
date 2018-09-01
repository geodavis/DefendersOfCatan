using DefendersOfCatan.Common;
using DefendersOfCatan.Models;
using System;
using System.Collections.Generic;
using DefendersOfCatan.DAL.DataModels;
using static DefendersOfCatan.Common.Enums;
using static DefendersOfCatan.Common.Globals;
using static DefendersOfCatan.Common.Constants;
using DefendersOfCatan.DAL.DataModels.Items;
using DefendersOfCatan.BusinessLogic.Repository;

namespace DefendersOfCatan.BusinessLogic
{
    
    public class GameInitializer
    {
        private ItemRepository itemRepo = new ItemRepository();

        public List<Tile> InitializeTiles()
        {
            var tiles = new List<Tile>();
            var randomResourceTypes = GetRandomResourceTileTypes();
            var i = 0;

            for (int x = 0; x <= TileLayout.GetUpperBound(0); x++) // row
            {
                for (int y = 0; y <= TileLayout.GetUpperBound(1); y++) // col
                {
                    var tile = new Tile { LocationX = y, LocationY = x, Type = (TileType)TileLayout[x, y], Name = "Tile" + x.ToString() + y.ToString() };
                
                    if (tile.Type == TileType.Resource)
                    {
                        tile.ResourceType = randomResourceTypes[i];
                        i++;
                    }
                    else
                    {
                        tile.ResourceType = ResourceType.NoResource;
                    }

                    tiles.Add(tile);
                }
            }
            return tiles;
        }

        public List<Player> InitializePlayers(Tile capitalTile)
        {
            var players = new List<Player>
            {
            new Player { Name = "GeoffR", Color = PlayerColor.Red, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources() },
            new Player { Name = "GeoffB", Color = PlayerColor.Blue, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources() },
            new Player { Name = "GeoffY", Color = PlayerColor.Yellow, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources()  },
            new Player { Name = "GeoffG", Color = PlayerColor.Green, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources()  }
            };

            foreach (var player in players)
            {
                capitalTile.Players.Add(player);
            }

            return players;
        }

        public List<Enemy>InitializeEnemies()
        {
            var enemies = new List<Enemy>();
            var playerColorValues = Enum.GetValues(typeof(PlayerColor));
            var random = new Random();

            for (int i = 0; i < NUM_ENEMIES; i++)
            {
                var randomPlayerColor = (PlayerColor)playerColorValues.GetValue(random.Next(playerColorValues.Length));
                var enemy = new Enemy { HasBarbarian = random.NextDouble() > 0.5, PlayerColor = randomPlayerColor };
                enemies.Add(enemy);
            }

            return enemies;
        }

        public void InitializeItems()
        {
            var items = new List<Item>();

            var resourceCost = new ResourceCost { ResourceType = 0, Qty = 2 };
            var itemCost = new List<ResourceCost>();
            itemCost.Add(resourceCost);
            var item = new Item { ItemType = 0, ItemName = "Item1", ItemCost = itemCost };
            items.Add(item);

            resourceCost = new ResourceCost { ResourceType = 1, Qty = 2 };
            itemCost = new List<ResourceCost>();
            itemCost.Add(resourceCost);
            item = new Item { ItemType = 1, ItemName = "Item2", ItemCost = itemCost };
            items.Add(item);

            itemRepo.AddItems(items);          
        }

        private List<PlayerResource> InitializePlayerResources()
        {
            var playerResources = new List<PlayerResource>();
            var playerResourceValues = Enum.GetValues(typeof(ResourceType));

            for (int i = 0; i < playerResourceValues.Length; i++)
            {
                playerResources.Add(new PlayerResource { ResourceType = (ResourceType)playerResourceValues.GetValue(i), Qty = 0 });
            }

            return playerResources;
        }

        private List<ResourceType> GetRandomResourceTileTypes()
        {
            var randomResourceTypes = new List<ResourceType>();
            var resourceValues = Enum.GetValues(typeof(ResourceType));
            var random = new Random();

            for (int i = 0; i < NUM_RESOURCE_TILES; i++)
            {
                var randomResourceType = (ResourceType)resourceValues.GetValue(random.Next(resourceValues.Length - 1)); // exclude no resource type
                randomResourceTypes.Add(randomResourceType);
            }

            return randomResourceTypes;
        }
    }
}


