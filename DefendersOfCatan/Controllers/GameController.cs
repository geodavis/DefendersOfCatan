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

        // GET: Game
        public ActionResult Index()
        {
            game.GameState = GameState.Initialization;

            game.Tiles = gameInitializer.InitializeTiles();
            var capitalTile = game.Tiles.Where(t => t.Type == TileType.Capital).Single();
            game.Players = gameInitializer.InitializePlayers(capitalTile);
            game.Enemies = gameInitializer.InitializeEnemies();

            db.Game.Add(game);
            db.SaveChanges();

            game.CurrentPlayer = game.Players[0];
            db.SaveChanges();

            return View();
        }

        [HttpGet]
        public JsonResult GetNextGameState()
        {
            var result = new ItemModel<string>();
            try
            {
                var game = db.GetSet<Game>().FirstOrDefault();
                var nextGameState = GetNextGameState(game.GameState);
                game.GameState = nextGameState;
                db.SaveChanges(); // save game state to DB
                result.Item = game.GameState.ToString();
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
            var result = new ItemModel<Items> { Item = new Items() };
            try
            {
                var resourceCost = new ResourceCost { ResourceType = 0, Qty = 2 };
                var itemCost = new List<ResourceCost>();
                itemCost.Add(resourceCost);
                var item = new Item { ItemType = 0, ItemName = "Item1", ItemCost = itemCost };
                result.Item.GameItems.Add(item);

                resourceCost = new ResourceCost { ResourceType = 1, Qty = 2 };
                itemCost = new List<ResourceCost>();
                itemCost.Add(resourceCost);
                item = new Item { ItemType = 1, ItemName = "Item2", ItemCost = itemCost };
                result.Item.GameItems.Add(item);

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
                var game = db.GetSet<Game>().FirstOrDefault();
                result.Item = game.Enemies;
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
        public JsonResult UpdateEnemy(UpdateEnemyTransfer data)
        {
            var result = new ItemModel<string>();

            try
            {
                var enemy = db.GetSet<Enemy>().Single(e => e.Id == data.Id);
                //enemy.CurrentTileName = data.CurrentHexName;
                enemy.HasBeenPlaced = data.HasBeenPlaced;
                enemy.BarbarianIndex = data.BarbarianIndex;
                db.SaveChanges();
                result.Item = "Successfully Saved Enemy!";
                return ReturnJsonResult(result);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Error = e.Message;
                result.Item = "Failure saving Enemy to DB!";
                return ReturnJsonResult(result);
            }

        }

        [HttpPost]
        public JsonResult AddEnemyToTile(EnemyTileTransfer data)
        {
            var result = new ItemModel<EnemyTileTransfer>();

            try
            {
                var tile = db.Tiles.Where(t => t.Id == data.TileId).Single(); ;
                var enemy = db.Enemies.Where(e => e.Id == data.EnemyId).Single();
                var playerIsOverrun = db.Players.Where(p => p.Color == enemy.PlayerColor).Single().IsOverrun; // Check if selected card can be placed on it's color, if not allow it to be placed anywhere

                if (tile.IsEnemyTile() && ((int)tile.Type == (int)enemy.PlayerColor) || playerIsOverrun)
                {
                    enemy.HasBeenPlaced = true;
                    tile.Enemy = enemy;

                    // Check if player is overrun - todo: Return player overrun value to update UI
                    var player = db.Players.Where(p => (int)p.Color == (int)tile.Type).Single();
                    player.IsOverrun = CheckIfPlayerIsOverrun(player);

                    db.SaveChanges();
                }
                else
                {
                    result.HasError = true;
                    result.Error = "Cannot place card on that color.";
                }

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

        private bool CheckIfPlayerIsOverrun(Player player)
        {
            var isOverrun = false;
            var tiles = db.Tiles.Where(t => (int)t.Type == (int)player.Color).ToList();

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

        [HttpPost]
        public JsonResult RemoveEnemy(EnemyTileTransfer data)
        {
            var result = new ItemModel<EnemyTileTransfer>();

            try
            {
                // Remove enemy
                var enemy = db.GetSet<Enemy>().Single(e => e.Id == data.EnemyId);
                enemy.IsRemoved = true;


                // Check if player is no longer overrun
                var enemyTile = db.Tiles.Where(t => t.Id == data.TileId).Single(); // get tile enemy was removed from
                var player = db.Players.Where(p => (int)p.Color == (int)enemyTile.Type).Single(); // get that player color
                player.IsOverrun = CheckIfPlayerIsOverrun(player);

                // Save data
                db.SaveChanges();
                result.Item = data; // todo: Return player overrun value to update UI
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

        [HttpPost]
        public JsonResult AddResourceToPlayer(PlayerResourceTransfer data)
        {
            var result = new ItemModel<PlayerResourceTransfer>();

            try
            {
                var playerResources = db.PlayerResources.Where(r => r.Player.Id == data.PlayerId & (int)r.ResourceType == data.ResourceType).Single();
                playerResources.Qty += 1;

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
                var game = db.Game.FirstOrDefault();
                var tiles = new List<Tile>();
                // Check current player for barbarian advancement
                foreach (var tile in game.Tiles)
                {
                    if (((int)game.CurrentPlayer.Color == (int)tile.Type) && tile.Enemy != null && tile.Enemy.HasBarbarian)
                    {
                        tile.Enemy.BarbarianIndex += 1;

                        // Reset barbarian index if it hits 3, and overrun the appropriate tile
                        if (tile.Enemy.BarbarianIndex == 2) // ToDo: change back to "3"
                        {
                            var overrunTile = SetOverrunTile(tile);
                            tiles.Add(overrunTile);
                            tile.Enemy.BarbarianIndex = 0; // Reset barbarian index
                        }

                        db.SaveChanges();
                        tiles.Add(tile);
                    }
                }

                result.Item = tiles;
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
        public JsonResult GetNeighbors(int tileId)
        {
            var result = new ItemModel<List<int>> { Item = new List<int>() };
            var tile = db.Tiles.Where(t => t.Id == tileId).Single();
            
            try
            {
                var neighboringTiles = GetNeighborTiles(tile);
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

        private List<Tile> GetNeighborTiles(Tile tile)
        {
            var neighboringTiles = new List<Tile>();
            //            element[x, y]
            //            neighbor1 = x + 1, y;
            //            neighbor2 = x - 1, y;
            //            neighbor3 = x, y + 1;
            //            neighbor4 = x, y - 1;
            //            neighbor5 = x + 1, y + 1; - NOT A HEX NEIGHBOR FOR EVEN ROW
            //            neighbor6 = x + 1, y - 1; - NOT A HEX NEIGHBOR FOR EVEN ROW
            //            neighbor7 = x - 1, y + 1; - NOT A HEX NEIGHBOR FOR ODD ROW
            //            neighbor8 = x - 1, y - 1; - NOT A HEX NEIGHBOR FOR ODD ROW

            // Each hex has 6 neighbors - the below 4 are always a neighbor, regardless if even or odd row
            AddNeighbor(tile.LocationX + 1, tile.LocationY, neighboringTiles);
            AddNeighbor(tile.LocationX - 1, tile.LocationY, neighboringTiles);
            AddNeighbor(tile.LocationX, tile.LocationY + 1, neighboringTiles);
            AddNeighbor(tile.LocationX, tile.LocationY - 1, neighboringTiles);
            
            // The next two neighbors will change depending on if it is an even or odd row
            if (tile.LocationY % 2 != 0) // odd row
            {
                AddNeighbor(tile.LocationX + 1, tile.LocationY + 1, neighboringTiles);
                AddNeighbor(tile.LocationX + 1, tile.LocationY - 1, neighboringTiles);
                
            }
            else // even row
            {
                AddNeighbor(tile.LocationX - 1, tile.LocationY + 1, neighboringTiles);
                AddNeighbor(tile.LocationX - 1, tile.LocationY - 1, neighboringTiles);
            }

            return neighboringTiles;
        }

        private void AddNeighbor(int x, int y, List<Tile> neighboringTiles)
        {
            if (db.Tiles.Where(t => t.LocationX == x && t.LocationY == y).Any())
            {
                neighboringTiles.Add(db.Tiles.Where(t => t.LocationX == x && t.LocationY == y).Single());
            }
        }

        private Tile SetOverrunTile(Tile tile)
        {
            var tileNumber = Globals.HexOverrunData[tile.LocationY, tile.LocationX];
            var overrunTile = GetNextOverrunTile(tile, tileNumber, 0);
            overrunTile.IsOverrun = true;
            db.SaveChanges();
            return overrunTile;
        }

        private Tile GetNextOverrunTile(Tile tile, string tileNumber, int tileCount)
        {
            // Get next tile in line
            // First, get neighbor with tile number
            var neighbors = GetNeighborTiles(tile);

            foreach (var neighbor in neighbors)
            {
                var overrunData = Globals.HexOverrunData[neighbor.LocationY, neighbor.LocationX];
                var splitOverrunData = overrunData.Split(',');

                if (splitOverrunData.Contains(tileNumber) == true && !neighbor.IsEnemyTile()) // if is enemy tile, do not consider that neighbor tile
                {
                    if (!neighbor.IsOverrun)
                    {
                        return neighbor;
                    }
                    else // neighbor tile is overrun
                    {
                        tileCount += 1;
                        if (tileCount == 4 || neighbor.Type == TileType.Capital)
                        {
                            Console.WriteLine("game over!"); // todo: second blue tile (46) flipping ends game - BUG; need to check entire row for count, not just the direction we are coming; do need a check end game state function
                        }
                        return GetNextOverrunTile(neighbor, tileNumber, tileCount); // if tile is overrun, move onto the next tile in line
                    }
                }
            }

            Console.WriteLine("Error getting next overrun tile!");
            return new Tile(); // Error condition here!!!
        }
        
        [HttpGet]
        public JsonResult MoveToNextPlayer()
        {
            var result = new ItemModel<int>();

            try
            {
                var game = db.Game.FirstOrDefault();
                var currentPlayer = game.CurrentPlayer;

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

                result.Item = (int) game.CurrentPlayer.Color;
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

        private GameState GetNextGameState(GameState gameState)
        {
            // ToDo: Ensure this logic works when wrapping to game state 1
            var currentEnumIndex = (int)gameState;
            var gameStateEnumValues = Enum.GetValues(typeof(GameState));
            return currentEnumIndex == gameStateEnumValues.Length ? (GameState)gameStateEnumValues.GetValue(1) : (GameState)gameStateEnumValues.GetValue(currentEnumIndex + 1);
        }


        public JsonResult ReturnJsonResult(object result)
        {
            return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

    }
}