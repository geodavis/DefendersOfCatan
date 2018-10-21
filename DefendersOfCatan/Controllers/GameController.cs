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
        private GameContext db = new GameContext();
        private GameInitializer gameInitializer = new GameInitializer();
        private Game game = new Game();
        private PlayerLogic playerLogic = new PlayerLogic();
        private TileLogic tileLogic = new TileLogic();
        private GameStateLogic gameStateLogic = new GameStateLogic();
        private EnemyLogic enemyLogic = new EnemyLogic();
        private DevelopmentLogic developmentLogic = new DevelopmentLogic();

        // GET: Game
        public ActionResult Index()
        {
            game.GameState = GameState.Initialization;

            game.Tiles = gameInitializer.InitializeTiles();
            var capitalTile = game.Tiles.Where(t => t.Type == TileType.Capital).Single();
            game.Players = gameInitializer.InitializePlayers(capitalTile);
            game.Enemies = gameInitializer.InitializeEnemies();
            gameInitializer.InitializeDevelopments();

            db.Game.Add(game);
            db.SaveChanges();

            game.CurrentPlayer = game.Players[0];
            db.SaveChanges();

            return View();
        }

        [HttpPost]
        public JsonResult SetSelectedEnemy(ClickedEnemyTransfer data)
        {
            var result = new ItemModel<ClickedEnemyTransfer>();
            try
            {
                var enemy = db.Enemies.Where(e => e.Id == data.EnemyId).Single();
                enemy.IsSelected = true;
                db.SaveChanges();
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
                var gameState = gameStateLogic.GetCurrentGameState();
                var enemy = db.Enemies.Where(e => e.Id == data.EnemyId).Single();
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
                        var currentPlayer = playerLogic.GetCurrentPlayer();
                        var tiles = db.Tiles.ToList();
                        var currentPlayerTile = tiles.Where(t => t.Players.Contains(currentPlayer)).Single();
                        var enemyTile = tiles.Where(t => t.Enemy != null && t.Enemy.Id == enemy.Id).Single();
                        result.Item.EnemyTileId = enemyTile.Id;
                        var neighborTiles = tileLogic.GetNeighborTiles(currentPlayerTile);

                        if (neighborTiles.Contains(enemyTile))
                        {
                            enemyLogic.RemoveEnemy(enemy, enemyTile);
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

                db.SaveChanges();

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
                var neighboringTiles = tileLogic.GetNeighborTiles(tileLogic.GetCurrentPlayerTile());
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
            var result = new ItemModel<ClickedTileTransfer>();
            result.Item = new ClickedTileTransfer();
            try
            {
                var gameState = gameStateLogic.GetCurrentGameState();
                var selectedTile = db.Tiles.Where(t => t.Id == data.ClickedTileId).Single();
                var currentPlayer = playerLogic.GetCurrentPlayer();
                result.Item.GameState = gameState.ToString();
                result.Item.ClickedTileId = selectedTile.Id;
                switch (gameState)
                {
                    case GameState.EnemyCard:
                        var selectedEnemy = db.Enemies.Where(e => e.IsSelected == true).Single();
                        enemyLogic.AddEnemyToTile(data);
                        result.Item.EnemyId = selectedEnemy.Id;
                        break;
                    case GameState.PlayerPlacePurchase:
                        // Get the item the player just purchased; If no item in inventory, return error message
                        var developmentType = developmentLogic.PlacePurchasedDevelopment(data.ClickedTileId);

                        // Save the item to the tile

                        // Take player resources

                        // Send return message to update UI - TODO: take this development type and place it on the tile in the UI on return message
                        result.Item.DevelopmentType = (int)developmentType;

                        break;
                    case GameState.PlayerMove:
                        result.Item.PlayerId = currentPlayer.Id;
                        if (!playerLogic.MovePlayerToTile(data.ClickedTileId))
                        {
                            result.HasError = true;
                            result.Error = "You cannot move to that tile.";
                        }
                        break;
                    case GameState.PlayerResourceOrFight:
                        var resourceType = selectedTile.ResourceType;
                        result.Item.ResourceType = (int)resourceType;
                        if (tileLogic.TileHasSettlement(data.ClickedTileId))
                        {
                            playerLogic.AddResourceToPlayer(resourceType);
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

                db.SaveChanges();
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
                result.Item = gameStateLogic.UpdateGameState().ToString();
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
                var game = db.GetSet<Game>().FirstOrDefault();
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
                var players = playerLogic.GetPlayers();
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
                result.Item = developmentLogic.GetDevelopments();
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
            var result = new ItemModel<int>();
            result.Item = (int)developmentType;

            try
            {
                if (!playerLogic.PurchaseDevelopment(developmentType))
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
                result.Item = enemyLogic.GetEnemies();
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
                var player = db.GetSet<Player>().Single(e => e.Id == data.Id);
                player.Health = data.Health;
                db.SaveChanges();
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
                var player = db.GetSet<Player>().Single(e => e.Id == data.Id);
                player.IsOverrun = data.IsOverrun;
                db.SaveChanges();
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
                var tile = db.Tiles.Where(t => t.Id == data.TileId).Single();
                var player = db.Players.Where(e => e.Id == data.PlayerId).Single();
                tile.Players.Add(player);
                db.SaveChanges();

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
                result.Item = enemyLogic.ExecuteEnemyMovePhase();
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
                var game = db.Game.FirstOrDefault();
                var currentPlayer = playerLogic.GetCurrentPlayer();

                switch (currentPlayer.Color)
                {
                    case PlayerColor.Red:
                        game.CurrentPlayer = game.Players.Where(p => p.Color == PlayerColor.Blue).Single();
                        break;
                    case PlayerColor.Blue:
                        game.CurrentPlayer = game.Players.Where(p => p.Color == PlayerColor.Yellow).Single();
                        break;
                    case PlayerColor.Yellow:
                        game.CurrentPlayer = game.Players.Where(p => p.Color == PlayerColor.Green).Single();
                        break;
                    case PlayerColor.Green:
                        game.CurrentPlayer = game.Players.Where(p => p.Color == PlayerColor.Red).Single();
                        break;
                    default:
                        Console.WriteLine("Error getting next player!");
                        break;
                }
                db.SaveChanges();

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