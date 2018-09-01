using DefendersOfCatan.Common;
using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using DefendersOfCatan.DAL.DataModels.Items;
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
        private ItemLogic itemLogic = new ItemLogic();

        // GET: Game
        public ActionResult Index()
        {
            game.GameState = GameState.Initialization;

            game.Tiles = gameInitializer.InitializeTiles();
            var capitalTile = game.Tiles.Where(t => t.Type == TileType.Capital).Single();
            game.Players = gameInitializer.InitializePlayers(capitalTile);
            game.Enemies = gameInitializer.InitializeEnemies();
            gameInitializer.InitializeItems();

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
                        //AddResourceToPlayer(currentPlayer, resourceType);
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
        public JsonResult GetItems()
        {
            var result = new ItemModel<List<Item>> { Item = new List<Item>() };
            try
            {
                result.Item = itemLogic.GetItems();
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
        public JsonResult PurchaseItem(int itemType)
        {
            var result = new ItemModel<Item> { Item = new Item() };
            try
            {
                // First check if player can purchase the item
                
                //var resourceCost = new ResourceCost { ResourceType = 0, Qty = 2 };
                //var itemCost = new List<ResourceCost>();
                //itemCost.Add(resourceCost);
                //var item = new Item { ItemType = 0, ItemName = "Item1", ItemCost = itemCost };
                //result.Item.GameItems.Add(item);

                //resourceCost = new ResourceCost { ResourceType = 1, Qty = 2 };
                //itemCost = new List<ResourceCost>();
                //itemCost.Add(resourceCost);
                //item = new Item { ItemType = 1, ItemName = "Item2", ItemCost = itemCost };
                //result.Item.GameItems.Add(item);

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

        private void AddResourceToPlayer(Player player, ResourceType resourceType)
        {
            var playerResources = db.PlayerResources.Where(r => r.Player.Id == player.Id && (int)r.ResourceType == (int)resourceType).Single();
            playerResources.Qty += 1;
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