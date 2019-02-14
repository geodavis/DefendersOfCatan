using DefendersOfCatan.Common;
using System;
using System.Collections.Generic;
using DefendersOfCatan.DAL.DataModels;
using static DefendersOfCatan.Common.Enums;
using static DefendersOfCatan.Common.Globals;
using static DefendersOfCatan.Common.Constants;
using DefendersOfCatan.BusinessLogic.Repository;
using System.Linq;

namespace DefendersOfCatan.BusinessLogic
{

    public interface IGameInitializer
    {
        void InitializeGame();
    }

    public class GameInitializer : IGameInitializer
    {
        private readonly IDevelopmentRepository _developmentRepo;
        private readonly IGameRepository _gameRepo;
        private readonly ITileLogic _tileLogic;

        public GameInitializer(IGameRepository gameRepo, IDevelopmentRepository developmentRepo, ITileLogic tileLogic)
        {
            _gameRepo = gameRepo;
            _developmentRepo = developmentRepo;
            _tileLogic = tileLogic;
        }

        public void InitializeGame()
        {
            var game = new Game();
            game.GameState = Enums.GameState.Initialization;
            game.Tiles = InitializeTiles();
            var capitalTile = game.Tiles.Single(t => t.Type == TileType.Capital);
            game.Players = InitializePlayers(capitalTile);
            game.Enemies = InitializeEnemies();
            _gameRepo.AddGame(game);

            game.Roads = InitializeRoads(game.Tiles);

            // Set current player after players have been added to the db
            game.CurrentPlayer = game.Players[0];
            _gameRepo.Save();

            InitializeDevelopments();
            InitializeCards();

        }

        private List<Tile> InitializeTiles()
        {
            var tiles = new List<Tile>();
            var randomResourceTypes = GetRandomResourceTileTypes();
            var i = 0;

            for (int x = 0; x <= TileLayout.GetUpperBound(0); x++) // row
            {
                for (int y = 0; y <= TileLayout.GetUpperBound(1); y++) // col
                {
                    var tile = new Tile { LocationX = y, LocationY = x, Type = (TileType)TileLayout[x, y], Name = "Tile" + x.ToString() + y.ToString() };
                    tile.Developments = new List<TileDevelopment>();

                    if (tile.Type == TileType.Resource)
                    {
                        //tile.ResourceType = ResourceType.Wool;
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

        private List<Road> InitializeRoads(List<Tile> tiles)
        {
            var roads = new List<Road>();

            foreach (var tile in tiles)
            {
                if (tile.Type == TileType.Resource || tile.Type == TileType.Capital)
                {
                    // Get neighbors to find adjoining tile
                    var neighbors = _tileLogic.GetNeighborTiles(tile);

                    // Loop each neighbor. If neighbor is resource tile type, add tile 2.                 
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.Type == TileType.Resource || neighbor.Type == TileType.Capital)
                        {
                            var road = new Road();
                            road.Tile1 = tile;
                            road.Tile2 = neighbor;
                            road.Angle = GetAngleBasedOnPosition(road.Tile1.LocationX, road.Tile1.LocationY, neighbor.LocationX, neighbor.LocationY); // todo: use position of neighbor tile to determine angle

                            // Check if road segment does not already exist. If not, add it to road list.
                            var roadExists = roads.Where((r => r.Tile1 == road.Tile1 || r.Tile1 == road.Tile2))
                                .Where((r => r.Tile2 == road.Tile1 || r.Tile2 == road.Tile2)).Any();

                            if (!roadExists)
                            {
                                roads.Add(road);
                            }
                        }
                    }
                }
            }

            return roads;
        }

        private int GetAngleBasedOnPosition(int tileX, int tileY, int neighborTileX, int neighborTileY)
        {
            var angle = 0;
            //            neighbor1 = x + 1, y; 90 degrees
            //            neighbor2 = x - 1, y; -90
            //            neighbor3 = x, y + 1; even y = 150, odd y = -150
            //            neighbor4 = x, y - 1; even y = 30, odd y = -30
            //            neighbor5 = x + 1, y + 1; - NOT A HEX NEIGHBOR FOR EVEN ROW 150
            //            neighbor6 = x + 1, y - 1; - NOT A HEX NEIGHBOR FOR EVEN ROW 30
            //            neighbor7 = x - 1, y + 1; - NOT A HEX NEIGHBOR FOR ODD ROW -150
            //            neighbor8 = x - 1, y - 1; - NOT A HEX NEIGHBOR FOR ODD ROW -30
            if ((neighborTileX == tileX + 1) && (tileY == neighborTileY))
            {
                angle = 90;
            }
            else if ((neighborTileX == tileX - 1) && (tileY == neighborTileY))
            {
                angle = -90;
            }
            else if ((neighborTileX == tileX) && (neighborTileY == tileY + 1))
            {
                angle = (tileY % 2 != 0) ? -150 : 150;                
            }
            else if ((neighborTileX == tileX) && (neighborTileY == tileY - 1))
            {
                angle = (tileY % 2 != 0) ? -30 : 30;
            }
            else if ((neighborTileX == tileX + 1) && (neighborTileY == tileY + 1))
            {
                angle = 150;
            }
            else if ((neighborTileX == tileX + 1) && (neighborTileY == tileY - 1))
            {
                angle = 30;
            }
            else if ((neighborTileX == tileX - 1) && (neighborTileY == tileY + 1))
            {
                angle = -150;
            }
            else if ((neighborTileX == tileX - 1) && (neighborTileY == tileY - 1))
            {
                angle = -30;
            }

            return angle;

        }

        private List<Player> InitializePlayers(Tile capitalTile)
        {
            var players = new List<Player>
            {
                new Player { Name = "GeoffR", Color = PlayerColor.Red, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources(), PlayerDevelopments = InitializePlayerDevelopments(), PlayerCards = InitializePlayerCards() },
                new Player { Name = "GeoffB", Color = PlayerColor.Blue, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources(), PlayerDevelopments = InitializePlayerDevelopments(), PlayerCards = InitializePlayerCards() },
                new Player { Name = "GeoffY", Color = PlayerColor.Yellow, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources(), PlayerDevelopments = InitializePlayerDevelopments(), PlayerCards = InitializePlayerCards() },
                new Player { Name = "GeoffG", Color = PlayerColor.Green, IsOverrun = false, Health = 5, PlayerResources = InitializePlayerResources(), PlayerDevelopments = InitializePlayerDevelopments(), PlayerCards = InitializePlayerCards() }
            };

            foreach (var player in players)
            {
                capitalTile.Players.Add(player);
            }

            return players;
        }

        private List<Enemy>InitializeEnemies()
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

        private void InitializeDevelopments()
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

            _developmentRepo.AddDevelopments(developments);          
        }

        private void InitializeCards()
        {
            var cards = new List<Card>();

            var card = new Card { CardType = CardType.a, Description = "a description"};
            cards.Add(card);

            card = new Card { CardType = CardType.b, Description = "b description" };
            cards.Add(card);

            card = new Card { CardType = CardType.c, Description = "c description" };
            cards.Add(card);

            card = new Card { CardType = CardType.d, Description = "d description" };
            cards.Add(card);
            
            _developmentRepo.AddCards(cards);
        }

        private List<PlayerResource> InitializePlayerResources()
        {
            var playerResources = new List<PlayerResource>();
            var playerResourceValues = Enum.GetValues(typeof(ResourceType));

            for (int i = 0; i < playerResourceValues.Length; i++)
            {
                playerResources.Add(new PlayerResource { ResourceType = (ResourceType)playerResourceValues.GetValue(i), Qty = 5 });
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

        private List<PlayerCard> InitializePlayerCards()
        {
            var playerCards = new List<PlayerCard>();
            var playerCardValues = Enum.GetValues(typeof(CardType));

            for (int i = 0; i < playerCardValues.Length; i++)
            {
                playerCards.Add(new PlayerCard { CardType = (CardType)playerCardValues.GetValue(i), Qty = 0 });
            }

            return playerCards;
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


