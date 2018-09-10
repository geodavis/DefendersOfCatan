using DefendersOfCatan.Common;
using DefendersOfCatan.Models;
using System;
using System.Collections.Generic;
using DefendersOfCatan.DAL.DataModels;
using static DefendersOfCatan.Common.Enums;
using static DefendersOfCatan.Common.Globals;
using static DefendersOfCatan.Common.Constants;
using DefendersOfCatan.BusinessLogic.Repository;

namespace DefendersOfCatan.BusinessLogic
{
    
    public class GameInitializer
    {
        private DevelopmentRepository developmentRepo = new DevelopmentRepository();

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
                new Player { Name = "GeoffR", Color = PlayerColor.Red, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources(), PlayerDevelopments = InitializePlayerDevelopments() },
                new Player { Name = "GeoffB", Color = PlayerColor.Blue, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources(), PlayerDevelopments = InitializePlayerDevelopments() },
                new Player { Name = "GeoffY", Color = PlayerColor.Yellow, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources(), PlayerDevelopments = InitializePlayerDevelopments() },
                new Player { Name = "GeoffG", Color = PlayerColor.Green, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources(), PlayerDevelopments = InitializePlayerDevelopments() }
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

        public void InitializeDevelopments()
        {
            var developments = new List<Development>();

            var resourceCost = new ResourceCost { ResourceType = 0, Qty = 1 };
            var developmentCost = new List<ResourceCost>();
            developmentCost.Add(resourceCost);
            var development = new Development { DevelopmentType = DevelopmentType.Road, DevelopmentName = "Dev1", DevelopmentCost = developmentCost };
            developments.Add(development);

            resourceCost = new ResourceCost { ResourceType = 1, Qty = 1 };
            developmentCost = new List<ResourceCost>();
            developmentCost.Add(resourceCost);
            development = new Development { DevelopmentType = DevelopmentType.Settlement, DevelopmentName = "Dev2", DevelopmentCost = developmentCost };
            developments.Add(development);

            resourceCost = new ResourceCost { ResourceType = 1, Qty = 1 };
            developmentCost = new List<ResourceCost>();
            developmentCost.Add(resourceCost);
            development = new Development { DevelopmentType = DevelopmentType.City, DevelopmentName = "Dev3", DevelopmentCost = developmentCost };
            developments.Add(development);

            resourceCost = new ResourceCost { ResourceType = 1, Qty = 1 };
            developmentCost = new List<ResourceCost>();
            developmentCost.Add(resourceCost);
            development = new Development { DevelopmentType = DevelopmentType.Walls, DevelopmentName = "Dev4", DevelopmentCost = developmentCost };
            developments.Add(development);

            resourceCost = new ResourceCost { ResourceType = 1, Qty = 1 };
            developmentCost = new List<ResourceCost>();
            developmentCost.Add(resourceCost);
            development = new Development { DevelopmentType = DevelopmentType.Card, DevelopmentName = "Dev5", DevelopmentCost = developmentCost };
            developments.Add(development);

            developmentRepo.AddDevelopments(developments);          
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

        private List<PlayerDevelopment> InitializePlayerDevelopments()
        {
            var playerDevelopments = new List<PlayerDevelopment>();
            var playerDevelopmentValues = Enum.GetValues(typeof(DevelopmentType));

            for (int i = 0; i < playerDevelopmentValues.Length; i++)
            {
                playerDevelopments.Add(new PlayerDevelopment { DevelopmentType = (DevelopmentType)playerDevelopmentValues.GetValue(i), Qty = 0 });
            }

            return playerDevelopments;
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


