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


namespace DefendersOfCatan.Controllers
{
    public class GameController : Controller
    {
        private readonly GameContext _db = new GameContext();
        private readonly GameInitializer _gameInitializer = new GameInitializer();
        private readonly Game _game = new Game();
        private readonly PlayerLogic _playerLogic = new PlayerLogic();
        private readonly TileLogic _tileLogic = new TileLogic();
        private readonly GameStateLogic _gameStateLogic = new GameStateLogic();
        private readonly EnemyLogic _enemyLogic = new EnemyLogic();
        private readonly DevelopmentLogic _developmentLogic = new DevelopmentLogic();

        // GET: Game
        public ActionResult Index()
        {
            _game.GameState = GameState.Initialization;

            _game.Tiles = _gameInitializer.InitializeTiles();
            var capitalTile = _game.Tiles.Single(t => t.Type == TileType.Capital);
            _game.Players = _gameInitializer.InitializePlayers(capitalTile);
            _game.Enemies = _gameInitializer.InitializeEnemies();
            _gameInitializer.InitializeDevelopments();

            _db.Game.Add(_game);
            _db.SaveChanges();

            _game.CurrentPlayer = _game.Players[0];
            _db.SaveChanges();

            return View();
        }

        [HttpPost]
        public JsonResult SetSelectedEnemy(ClickedEnemyTransfer data)
        {
            var result = new ItemModel<ClickedEnemyTransfer>();
            try
            {
                var enemy = _db.Enemies.Single(e => e.Id == data.EnemyId);
                enemy.IsSelected = true;
                _db.SaveChanges();
                result.Item = data;
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
                var enemy = _db.Enemies.Single(e => e.Id == data.EnemyId);
                result.Item = new ClickedEnemyTransfer() { EnemyId = enemy.Id, GameState = gameState.ToString() };
                switch (gameState)
                {
                    case GameState.EnemyCard:
                        enemy.IsSelected = true;
                        break;
                    case GameState.PlayerMove:
                        // ToDo: Implement error handling (pass error to client)
                        break;
                    case GameState.PlayerResourceOrFight:
                        var currentPlayer = _playerLogic.GetCurrentPlayer();
                        var tiles = _db.Tiles.ToList();
                        var currentPlayerTile = tiles.Where(t => t.Players.Contains(currentPlayer)).Single();
                        var enemyTile = tiles.Where(t => t.Enemy != null && t.Enemy.Id == enemy.Id).Single();
                        result.Item.EnemyTileId = enemyTile.Id;
                        var neighborTiles = _tileLogic.GetNeighborTiles(currentPlayerTile);

                        if (neighborTiles.Contains(enemyTile))
                        {
                            _enemyLogic.RemoveEnemy(enemy, enemyTile);
                        }
                        else
                        {
                            result.HasError = true;
                            result.Error = "You cannot reach that enemy.";
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
                var selectedTile = _db.Tiles.Single(t => t.Id == data.ClickedTileId);
                var currentPlayer = _playerLogic.GetCurrentPlayer();
                result.Item.GameState = gameState.ToString();
                result.Item.ClickedTileId = selectedTile.Id;
                result.Item.PlayerId = currentPlayer.Id;
                switch (gameState)
                {
                    case GameState.InitialPlacement:
                        result.Item.DevelopmentType = (int)_developmentLogic.PlaceInitialSettlement(data.ClickedTileId);
                        break;
                    case GameState.EnemyCard:
                        var selectedEnemy = _db.Enemies.Single(e => e.IsSelected);
                        _enemyLogic.AddEnemyToTile(data);
                        result.Item.EnemyId = selectedEnemy.Id;
                        break;
                    case GameState.PlayerPurchase:
                        // Get the item the player just purchased; If no item in inventory, return error message
                        var developmentType = _developmentLogic.PlacePurchasedDevelopment(data.ClickedTileId);
                        result.Item.DevelopmentType = (int)developmentType;

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
                var game = _db.GetSet<Game>().FirstOrDefault();
                result.Item = game.Tiles;
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
                var players = _playerLogic.GetPlayers();
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
            var result = new ItemModel<int>
            {
                Item = (int)developmentType
            };

            try
            {
                if (!_playerLogic.PurchaseDevelopment(developmentType))
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
                result.Item = _enemyLogic.GetEnemies();
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
                var tile = _db.Tiles.Single(t => t.Id == data.TileId);
                var player = _db.Players.Single(e => e.Id == data.PlayerId);
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
            var result = new ItemModel<List<Tile>> { Item = new List<Tile>() };

            // In this phase, we progress any barbarians tied to the current players tiles
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
                var game = _db.Game.FirstOrDefault();
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