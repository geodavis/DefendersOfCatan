using DefendersOfCatan.Common;
using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static DefendersOfCatan.Common.Enums;
using static DefendersOfCatan.Common.Constants;
using DefendersOfCatan.BusinessLogic;
using DefendersOfCatan.BusinessLogic.Repository;
using System.Globalization;

namespace DefendersOfCatan.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameContext _db;
        private readonly IGameInitializer _gameInitializer;
        private readonly Game _game = new Game();
        private readonly IPlayerLogic _playerLogic;
        private readonly ITileLogic _tileLogic;
        private readonly IGameStateLogic _gameStateLogic;
        private readonly IEnemyLogic _enemyLogic;
        private readonly IDevelopmentLogic _developmentLogic;
        private readonly IPlayerRepository _playerRepo;
        private readonly IEnemyRepository _enemyRepo;
        private readonly ITileRepository _tileRepo;

        public GameController(IGameInitializer gameInitializer, IGameContext db, IPlayerRepository playerRepo,
            IEnemyRepository enemyRepo, ITileRepository tileRepo, IDevelopmentLogic developmentLogic,
            IGameStateLogic gameStateLogic, IPlayerLogic playerLogic, ITileLogic tileLogic, IEnemyLogic enemyLogic)
        {
            _gameInitializer = gameInitializer;
            _db = db;
            _playerRepo = playerRepo;
            _enemyRepo = enemyRepo;
            _tileRepo = tileRepo;
            _developmentLogic = developmentLogic;
            _gameStateLogic = gameStateLogic;
            _playerLogic = playerLogic;
            _tileLogic = tileLogic;
            _enemyLogic = enemyLogic;
        }

        // GET: Game
        public ActionResult Index()
        {
            _gameInitializer.InitializeGame();


            return View();
        }

        [HttpGet]
        public JsonResult GetInitialSettlementPlacement()
        {
            var result = new ItemModel<List<Tile>> { Item = new List<Tile>() };

            try
            {
                result.Item = _tileRepo.GetPlaceableDevelopments(DevelopmentType.Settlement).Tiles;
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpPost]
        public JsonResult ExecuteEnemyClickedActions(ClickedEnemyTransfer data)
        {
            var result = new ItemModel<ClickedEnemyTransfer>();
            try
            {
                var gameState = _gameStateLogic.GetCurrentGameState();
                var enemy = _enemyRepo.GetEnemy(data.EnemyId);
                result.Item = new ClickedEnemyTransfer { EnemyId = enemy.Id, GameState = gameState.ToString() };
                switch (gameState)
                {
                    case GameState.EnemyCard:
                        var setSelectedEnemy = _enemyRepo.SetSelectedEnemy(data.EnemyId);
                        if (!setSelectedEnemy)
                        {
                            result.HasError = true;
                            result.Error = "Already selected a card!";
                        }
                        break;
                    case GameState.PlayerMove:
                        // ToDo: Implement error handling (pass error to client)
                        break;
                    case GameState.PlayerResourceOrFight:

                        break;
                    default:
                        Console.WriteLine("Error getting game state!");
                        break;
                }

                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult GetCurrentPlayerNeighbors()
        {
            var result = new ItemModel<List<int>> { Item = new List<int>() };

            try
            {
                var neighboringTiles = _tileLogic.GetNeighborTiles(_tileLogic.GetCurrentPlayerTile());
                foreach (var neighboringTile in neighboringTiles)
                {
                    result.Item.Add(neighboringTile.Id);
                }

                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpPost]
        public JsonResult ExecuteTileClickedActions(ClickedTileTransfer data)
        {
            var result = new ItemModel<ClickedTileTransfer>
            {
                Item = new ClickedTileTransfer()
            };
            try
            {
                var gameState = _gameStateLogic.GetCurrentGameState();
                var selectedTile = _tileRepo.GetTileById(data.ClickedTileId);
                var currentPlayer = _playerRepo.GetCurrentPlayer();
                result.Item.GameState = gameState.ToString();
                result.Item.ClickedTileId = selectedTile.Id;
                result.Item.PlayerId = currentPlayer.Id;
                switch (gameState)
                {
                    case GameState.InitialPlacement:
                        result.Item.DevelopmentType = (int)_developmentLogic.PlaceInitialSettlement(data.ClickedTileId);
                        break;
                    case GameState.EnemyCard:
                        var transfer = _enemyLogic.AddEnemyToTile(data);
                        result.Item.EnemyId = transfer.EnemyId;
                        result.Item.IsOverrun = transfer.IsOverrun;
                        result.Item.PlayerId = transfer.PlayerId; // overrun player id

                        break;
                    case GameState.PlayerPurchase:
                        // Get the item the player just purchased; If no item in inventory, return error message
                        //var developmentType = _developmentLogic.PlacePurchasedDevelopment(data.ClickedTileId);
                        //result.Item.DevelopmentType = (int)developmentType;

                        break;
                    case GameState.PlayerMove:
                        result.Item.PlayerId = currentPlayer.Id;
                        if (!_playerLogic.MovePlayerToTile(data.ClickedTileId))
                        {
                            result.HasError = true;
                            result.Error = "You cannot move to that tile.";
                        }
                        break;
                    case GameState.PlayerResourceOrFight:

                        break;
                    default:
                        Console.WriteLine("Error getting game state!");
                        break;
                }

                _db.SaveChanges();
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpPost]
        public JsonResult MovePlayer(ClickedPlaceableTransfer data)
        {
            var result = new ItemModel<ClickedTileTransfer>
            {
                Item = new ClickedTileTransfer()
            };
            try
            {
                var gameState = _gameStateLogic.GetCurrentGameState(); // ToDo: validate correct game state
                var currentPlayer = _playerRepo.GetCurrentPlayer();
                result.Item.PlayerId = currentPlayer.Id;
                result.Item.ClickedTileId = data.ParentTileId;
                if (!_playerLogic.MovePlayerToTile(data.ParentTileId))
                {
                    result.HasError = true;
                    result.Error = "You cannot move to that tile.";
                }
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpPost]
        public JsonResult AddResourceToPlayer(ClickedPlaceableTransfer data)
        {
            var result = new ItemModel<PlayerResourceTransfer>
            {
                Item = new PlayerResourceTransfer()
            };
            try
            {
                var gameState = _gameStateLogic.GetCurrentGameState(); // ToDo: validate correct game state
                var tile = _tileRepo.GetTileById(data.ParentTileId);
                var tiles = _developmentLogic.GetPlayerPathTilesWithSettlements(); // Get valid tiles to collect resource from
                tiles.Add(_tileRepo.GetCurrentPlayerTile().Id); // Current player tile - ToDo: only if settlement exists on this tile

                if (tiles.Contains(tile.Id))
                {
                    _playerRepo.AddResourceToCurrentPlayer(tile.ResourceType);
                }
                else
                {
                    result.HasError = true;
                    result.Error = "Unable to collect resource from this tile.";
                }
                result.Item.ResourceType = tile.ResourceType;
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }
        [HttpPost]
        public JsonResult AttackEnemy(ClickedPlaceableTransfer data)
        {
            var result = new ItemModel<EnemyFightTransfer> { Item = new EnemyFightTransfer() };
            try
            {
                var enemy = _tileRepo.GetEnemyByTileId(data.ParentTileId);
                if (_enemyLogic.IsEnemyTileNeighbor(enemy.Id))
                {
                    var diceRolls = _enemyLogic.RollDice();

                    if (diceRolls.First() > 0) // ToDo: Change this value based on rules
                    {
                        var enemyFightTransfer = _enemyLogic.RemoveEnemy(enemy.Id);
                        result.Item.OverrunPlayerId = enemyFightTransfer.OverrunPlayerId;
                        result.Item.EnemyTile = enemyFightTransfer.EnemyTile;

                    }
                    else
                    {
                        result.HasError = true;
                        result.Error = "Missed on the roll.";
                    }
                }
                else
                {
                    result.HasError = true;
                    result.Error = "Enemy not on neighboring tile.";
                }
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpPost]
        public JsonResult PushBarbariansBack(ClickedPlaceableTransfer data)
        {
            var result = new ItemModel<EnemyMoveTransfer> { Item = new EnemyMoveTransfer() };
            try
            {
                var tile = _tileRepo.GetTileById(data.ParentTileId);
                result.Item.BarbarianTiles = _enemyLogic.PushBarbarianBack(tile.Type);
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpPost]
        public JsonResult RemoveEnemy(ClickedPlaceableTransfer data)
        {
            var result = new ItemModel<EnemyFightTransfer> { Item = new EnemyFightTransfer() };
            try
            {
                var enemy = _tileRepo.GetEnemyByTileId(data.ParentTileId);
                var enemyFightTransfer = _enemyLogic.RemoveEnemy(enemy.Id);
                result.Item.OverrunPlayerId = enemyFightTransfer.OverrunPlayerId;
                result.Item.EnemyTile = enemyFightTransfer.EnemyTile;
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpPost]
        public JsonResult PlaceRoad(PlaceRoadTransfer data)
        {
            var result = new ItemModel<PlaceRoadTransfer>
            {
                Item = new PlaceRoadTransfer()
            };
            try
            {
                var angle = _developmentLogic.PlacePurchasedRoad(data.Tile1Id, data.Tile2Id);
                result.Item.Tile1Id = data.Tile1Id;
                result.Item.Tile2Id = data.Tile2Id;
                result.Item.Angle = angle;
                result.Item.GameState = _gameStateLogic.GetCurrentGameState().ToString();
                result.Item.DevelopmentType = (int)DevelopmentType.Road;
                //result.Item.Paths = _developmentLogic.GetRoadPaths();
                //var test = _developmentLogic.GetPathTilesWithSettlements(result.Item.Paths);
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpPost]
        public JsonResult ExecutePlaceableClickedActions(ClickedPlaceableTransfer data)
        {
            // ToDo: ................... build this method correctly
            var result = new ItemModel<ClickedTileTransfer>
            {
                Item = new ClickedTileTransfer()
            };
            try
            {
                var gameState = _gameStateLogic.GetCurrentGameState();
                var selectedTile = _tileRepo.GetTileById(data.ParentTileId);
                var currentPlayer = _playerRepo.GetCurrentPlayer();

                result.Item.GameState = gameState.ToString();
                result.Item.ClickedTileId = selectedTile.Id;
                result.Item.PlayerId = currentPlayer.Id;
                result.Item.DevelopmentType = (int)data.developmentType;

                switch (gameState)
                {
                    case GameState.InitialPlacement:
                        _developmentLogic.PlaceInitialSettlement(data.ParentTileId);
                        break;
                    case GameState.EnemyCard:

                        break;
                    case GameState.PlayerPurchase:
                        // Get the item the player just purchased; If no item in inventory, return error message
                        _developmentLogic.PlacePurchasedDevelopment(data.ParentTileId);
                        break;
                    default:
                        Console.WriteLine("Game state not implemented for placeable clicked action.");
                        break;
                }

                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult GetNextGameState()
        {
            var result = new ItemModel<string>();
            try
            {
                result.Item = _gameStateLogic.UpdateGameState().ToString();
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult GetBoardData()
        {
            var result = new ItemModel<List<Tile>> { Item = new List<Tile>() };
            try
            {
                result.Item = _tileRepo.GetTiles();
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult GetPlayers()
        {
            var result = new ItemModel<List<Player>> { Item = new List<Player>() };
            try
            {
                var players = _playerRepo.GetPlayers();
                result.Item = players;
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult GetDevelopments()
        {
            var result = new ItemModel<List<DevelopmentTransfer>> { Item = new List<DevelopmentTransfer>() };
            try
            {
                result.Item = _developmentLogic.GetDevelopments();
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult GetCards()
        {
            var result = new ItemModel<List<CardTransfer>> { Item = new List<CardTransfer>() };
            try
            {
                result.Item = _developmentLogic.GetCards();
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult PurchaseDevelopment(DevelopmentType developmentType)
        {
            var result = new ItemModel<PurchaseDevelopmentTransfer> { Item = new PurchaseDevelopmentTransfer() };

            try
            {
                var canPurchase = _playerLogic.CurrentPlayerCanPurchaseDevelopment(developmentType);
                if (canPurchase)
                {
                    result.Item.DevelopmentType = developmentType;
                    _playerLogic.PurchaseDevelopment(developmentType);
                    if (developmentType == DevelopmentType.Card)
                    {
                        result.Item.CardType = _developmentLogic.AddRandomCardToPlayer();
                    }
                    var transfer = _tileRepo.GetPlaceableDevelopments(developmentType);
                    result.Item.Tiles = transfer.Tiles;
                    result.Item.Roads = transfer.Roads;
                }
                else
                {
                    result.HasError = true;
                    result.Error = "Cannot purchase this item.";
                }

                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult GetCardPlaceables(CardType cardType)
        {
            var result = new ItemModel<CardPlaceableTransfer> { Item = new CardPlaceableTransfer() };

            try
            {
                result.Item.CardType = cardType;
                switch (cardType)
                {
                    case CardType.BarbarianBack:
                        result.Item.TileIds = _tileRepo.GetEnemyTileIds(); // ToDo: filter by where enemy exists
                        break;
                    case CardType.EnemyRemove:
                        result.Item.TileIds = _tileRepo.GetTilesWithEnemyIds();
                        break;
                    case CardType.PlayerMove:
                        _playerRepo.SetCanMoveToAnyTile(true);
                        result.Item.TileIds = _tileRepo.GetPlayerMoveableTileIds(); // ToDo: Ensure move phase only
                        break;
                    case CardType.FreeDevelopment:
                        _playerRepo.SetCanPurchaseAnyDevelopment(true); // ToDo: Ensure purchase phase only
                        break;
                    default:
                        Console.WriteLine("Error getting next player!");
                        break;
                }
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult GetEnemies()
        {
            var result = new ItemModel<List<Enemy>> { Item = new List<Enemy>() };
            try
            {
                result.Item = _enemyRepo.GetEnemies();
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        //[HttpPost]
        //public JsonResult UpdateEnemy(UpdateEnemyTransfer data)
        //{
        //    var result = new ItemModel<string>();

        //    try
        //    {
        //        enemyLogic.UpdateEnemy(data);
        //        result.Item = "Successfully Saved Enemy!";
        //        return ReturnJsonResult(result);
        //    }
        //    catch (Exception e)
        //    {
        //        result.HasError = true;
        //        result.Error = e.Message;
        //        result.Item = "Failure saving Enemy to DB!";
        //        return ReturnJsonResult(result);
        //    }

        //}

        [HttpPost]
        public JsonResult UpdatePlayerHealth(PlayerHealthTransfer data)
        {
            var result = new ItemModel<string>();

            try
            {
                var player = _db.GetSet<Player>().Single(e => e.Id == data.Id);
                player.Health = data.Health;
                _db.SaveChanges();
                result.Item = "Successfully Updated Player Health!";
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                result.Item = "Failure saving Player Health to DB!";
                return ReturnJsonResult(result);
            }

        }

        [HttpPost]
        public JsonResult SetPlayerOverrun(PlayerOverrunTransfer data)
        {
            var result = new ItemModel<string>();

            try
            {
                var player = _db.GetSet<Player>().Single(e => e.Id == data.Id);
                player.IsOverrun = data.IsOverrun;
                _db.SaveChanges();
                result.Item = "Successfully Updated Player Overrun!";
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                result.Item = "Failure saving Player Overrun to DB!";
                return ReturnJsonResult(result);
            }

        }

        [HttpGet]
        public JsonResult ExecuteEnemyMovePhase()
        {
            var result = new ItemModel<EnemyMoveTransfer>();

            // In this phase, we progress any barbarians tied to the current players tiles
            // ToDo: Barbarians start at strength 1, and progress +1 every time they hit;
            // to defeat, you must roll at least their strength;
            // they also do more damage (take out more developments) when they hit as they get stronger
            try
            {
                result.Item = _enemyLogic.ExecuteEnemyMovePhase();
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        [HttpGet]
        public JsonResult MoveToNextPlayer()
        {
            var result = new ItemModel<int>();

            try
            {
                var game = _db.GetSet<Game>().FirstOrDefault();
                var currentPlayer = _playerLogic.GetCurrentPlayer();

                switch (currentPlayer.Color)
                {
                    case PlayerColor.Red:
                        game.CurrentPlayer = game.Players.Single(p => p.Color == PlayerColor.Blue);
                        break;
                    case PlayerColor.Blue:
                        game.CurrentPlayer = game.Players.Single(p => p.Color == PlayerColor.Yellow);
                        break;
                    case PlayerColor.Yellow:
                        game.CurrentPlayer = game.Players.Single(p => p.Color == PlayerColor.Green);
                        break;
                    case PlayerColor.Green:
                        game.CurrentPlayer = game.Players.Single(p => p.Color == PlayerColor.Red);
                        break;
                    default:
                        Console.WriteLine("Error getting next player!");
                        break;
                }
                _db.SaveChanges();

                result.Item = (int)game.CurrentPlayer.Color;
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                result.Item = -1;
                return ReturnJsonResult(result);
            }

        }

        [HttpGet]
        public JsonResult GetResourceOrFightTiles()
        {
            var result = new ItemModel<PlayerResourceOrFightTransfer> { Item = new PlayerResourceOrFightTransfer() };
            try
            {
                result.Item.ResourceTiles = _developmentLogic.GetPlayerPathTilesWithSettlements(); // All tiles with settlement along road paths
                result.Item.ResourceTiles.Add(_tileRepo.GetCurrentPlayerTile().Id); // Current player tile - ToDo: only if settlement exists on this tile
                result.Item.FightTiles = _enemyLogic.GetNeighborEnemyTileIds();
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                return ReturnJsonResult(result);
            }
        }

        public JsonResult ReturnJsonResult(object result)
        {
            return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

    }
}