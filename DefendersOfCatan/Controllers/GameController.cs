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
                        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture);
                        System.Diagnostics.Debug.WriteLine("Set selected enemy id: " + data.EnemyId + " - " + timestamp);
                        break;
                    case GameState.PlayerMove:
                        // ToDo: Implement error handling (pass error to client)
                        break;
                    case GameState.PlayerResourceOrFight:
                        var enemyFightTransfer =_enemyLogic.RemoveEnemy(data.EnemyId);
                        result.Item.DiceRolls = enemyFightTransfer.DiceRolls;
                        if (enemyFightTransfer.CanReach)
                        {
                            result.Item.EnemyTileId = enemyFightTransfer.EnemyTile.Id;
                            result.Item.EnemyHit = enemyFightTransfer.EnemyHit;
                        }
                        else
                        {
                            result.HasError = true;
                            result.Error = enemyFightTransfer.Message;
                        }

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
                        result.Item.EnemyId = _enemyLogic.AddEnemyToTile(data).Id;
                        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture);
                        System.Diagnostics.Debug.WriteLine("Place selected enemy tile id: " + data.ClickedTileId + " - " + timestamp);

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
                        var resourceType = selectedTile.ResourceType;
                        result.Item.ResourceType = (int)resourceType;
                        if (_tileLogic.TileHasSettlement(data.ClickedTileId))
                        {
                            _playerLogic.AddResourceToPlayer(resourceType);
                        }
                        else
                        {
                            result.HasError = true;
                            result.Error = "No settlement on tile.";
                        }
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
                result.Item.DevelopmentType = (int) DevelopmentType.Road;
                _developmentLogic.GetRoadPaths();
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
                result.Item.DevelopmentType = (int) data.developmentType;

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
        public JsonResult PurchaseDevelopment(DevelopmentType developmentType)
        {
            var result = new ItemModel<PurchaseDevelopmentTransfer> { Item = new PurchaseDevelopmentTransfer() };

            try
            {
                result.Item.CanPurchase = _playerLogic.PurchaseDevelopment(developmentType);
                result.Item.DevelopmentType = developmentType;
                var transfer = _tileRepo.GetPlaceableDevelopments(developmentType);
                result.Item.Tiles = transfer.Tiles;
                result.Item.Roads = transfer.Roads;

                if (!result.Item.CanPurchase)
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

        [HttpPost]
        public JsonResult MovePlayerToTile(PlayerTileTransfer data)
        {
            var result = new ItemModel<PlayerTileTransfer>();

            try
            {
                var tile = _db.GetSet<Tile>().Single(t => t.Id == data.TileId);
                var player = _db.GetSet<Player>().Single(e => e.Id == data.PlayerId);
                tile.Players.Add(player);
                _db.SaveChanges();

                result.Item = data;
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                result.Item = data;
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

        public JsonResult ReturnJsonResult(object result)
        {
            return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

    }
}