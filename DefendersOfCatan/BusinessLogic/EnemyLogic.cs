using DefendersOfCatan.BusinessLogic.Repository;
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
                if (((int)game.CurrentPlayer.Color == (int)tile.Type) && tile.Enemy != null && tile.Enemy.HasBarbarian)
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
    }
}