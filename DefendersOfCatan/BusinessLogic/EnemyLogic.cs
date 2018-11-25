using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.Common;
using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.BusinessLogic
{
    public class EnemyLogic
    {
        private EnemyRepository enemyRepo = new EnemyRepository();
        private TileLogic tileLogic = new TileLogic();
        private TileRepository tileRepo = new TileRepository();
        private PlayerLogic playerLogic = new PlayerLogic();
        private PlayerRepository playerRepo = new PlayerRepository();


        public List<Enemy> GetEnemies()
        {
            return enemyRepo.GetEnemies();
        }

        public void UpdateEnemy(UpdateEnemyTransfer enemyTransfer)
        {
            enemyRepo.UpdateEnemy(enemyTransfer);
        }

        public List<Tile> ExecuteEnemyMovePhase()
        {
            var game = enemyRepo.GetGame();
            var tiles = new List<Tile>();

            // Check current player for barbarian advancement
            foreach (var tile in game.Tiles)
            {
                if ((int)game.CurrentPlayer.Color == (int)tile.Type && tile.Enemy != null && tile.Enemy.HasBarbarian)
                {
                    var barbarianIndex = tile.Enemy.BarbarianIndex + 1;

                    // Reset barbarian index if it hits 3, and overrun the appropriate tile
                    if (barbarianIndex == 2) // put this back to 3
                    {
                        var overrunTile = tileLogic.SetOverrunTile(tile);
                        tiles.Add(overrunTile);
                        barbarianIndex = 0; // Reset barbarian index
                    }

                    enemyRepo.UpdateBarbarianIndex(tile.Enemy, barbarianIndex);
                    tiles.Add(tile);
                }
            }

            return tiles;
        }

        public void AddEnemyToTile(ClickedTileTransfer tileTransfer)
        {
            var tile = tileRepo.GetTileById(tileTransfer.ClickedTileId);
            var enemy = enemyRepo.GetSelectedEnemy();
            if (CanEnemyBeAddedToTile(enemy, tile))
            {
                tileRepo.AddEnemyToTile(enemy.Id, tile.Id);
                enemyRepo.SetEnemyPlaced(enemy);
                var player = playerRepo.GetPlayerBasedOnColor(tile.PlayerColor);
                if (playerLogic.CheckIfPlayerIsOverrun(player))
                {
                    playerRepo.SetPlayerOverrun(player, true);
                }
            }
            else
            {
                // ToDo: return error if validation fails
            }

        }
        private bool CanEnemyBeAddedToTile(Enemy enemy, Tile tile)
        {
            var playerIsOverrun = playerRepo.GetPlayerOverrunBasedOnPlayerColor(enemy.PlayerColor);
            return tile.IsEnemyTile() && ((int)tile.Type == (int)enemy.PlayerColor) || playerIsOverrun ? true : false;
        }

        public void RemoveEnemy(Enemy enemy, Tile tile)
        {
            enemyRepo.RemoveEnemy(enemy);

            // Check if player is no longer overrun
            var player = playerRepo.GetPlayerBasedOnColor((Enums.PlayerColor)tile.Type); // get the player color of the tile
            if (playerLogic.CheckIfPlayerIsOverrun(player))
            {
                playerRepo.SetPlayerOverrun(player, true);
            }
        }
    }
}