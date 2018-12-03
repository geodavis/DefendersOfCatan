﻿using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.Common;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System.Collections.Generic;

namespace DefendersOfCatan.BusinessLogic
{
    public interface IEnemyLogic
    {
        Enemy AddEnemyToTile(ClickedTileTransfer tileTransfer);
        void RemoveEnemy(Enemy enemy, Tile tile);
        List<Tile> ExecuteEnemyMovePhase();
    }
    public class EnemyLogic : IEnemyLogic
    {
        private IEnemyRepository _enemyRepo;
        private ITileLogic _tileLogic;
        private ITileRepository _tileRepo;
        private IPlayerLogic _playerLogic;
        private IPlayerRepository _playerRepo;

        public EnemyLogic(IEnemyRepository enemyRepo, ITileLogic tileLogic, ITileRepository tileRepo, IPlayerLogic playerLogic, IPlayerRepository playerRepo)
        {
            _enemyRepo = enemyRepo;
            _tileLogic = tileLogic;
            _tileRepo = tileRepo;
            _playerLogic = playerLogic;
            _playerRepo = playerRepo;

        }

        public List<Enemy> GetEnemies()
        {
            return _enemyRepo.GetEnemies();
        }

        public void UpdateEnemy(UpdateEnemyTransfer enemyTransfer)
        {
            _enemyRepo.UpdateEnemy(enemyTransfer);
        }

        public EnemyMoveTransfer ExecuteEnemyMovePhase()
        {
            var enemyMoveTransfer = new EnemyMoveTransfer { BarbarianTiles = new List<Tile>(), OverrunTiles = new List<Tile>(), OverrunDevelopments = new List<Development>() };
            var tiles = new List<Tile>();

            // Check current player for barbarian advancement
            foreach (var tile in _tileRepo.GetTiles())
            {
                if ((int)_playerRepo.GetCurrentPlayer().Color == (int)tile.Type && tile.Enemy != null && tile.Enemy.HasBarbarian)
                {
                    var barbarianIndex = tile.Enemy.BarbarianIndex + 1;
                    var barbarianStrength = tile.Enemy.Strength;
                    // Reset barbarian index if it hits 3, and overrun the appropriate tile
                    if (barbarianIndex == 2) // put this back to 3
                    {
                        var overrunTile = _tileLogic.SetOverrunTile(tile);
                        enemyMoveTransfer.OverrunTiles.Add(overrunTile); // Add tile to overrun
                        barbarianIndex = 0; // Reset barbarian index
                        if (barbarianStrength != 5) // Barbarian strength starts at 1 and has a max of 5
                        {
                            barbarianStrength++;
                        }
                    }

                    _enemyRepo.UpdateBarbarian(tile.Enemy, barbarianIndex, barbarianStrength);
                    enemyMoveTransfer.BarbarianTiles.Add(tile); // Add tile to update barbarian index of
                }
            }

            return enemyMoveTransfer; // ToDo: Return any developments removed by barbarian attack so that UI can be updated
        }

        public Enemy AddEnemyToTile(ClickedTileTransfer tileTransfer)
        {
            var tile = _tileRepo.GetTileById(tileTransfer.ClickedTileId);
            var enemy = _enemyRepo.GetSelectedEnemy();
            if (CanEnemyBeAddedToTile(enemy, tile))
            {
                _tileRepo.AddEnemyToTile(enemy.Id, tile.Id);
                _enemyRepo.SetEnemyPlaced(enemy);
                var player = _playerRepo.GetPlayerBasedOnColor(tile.PlayerColor);
                if (_playerLogic.CheckIfPlayerIsOverrun(player))
                {
                    _playerRepo.SetPlayerOverrun(player, true);
                }
            }
            else
            {
                // ToDo: return error if validation fails
            }

            return enemy;

        }
        private bool CanEnemyBeAddedToTile(Enemy enemy, Tile tile)
        {
            var playerIsOverrun = _playerRepo.GetPlayerOverrunBasedOnPlayerColor(enemy.PlayerColor);
            return tile.IsEnemyTile() && ((int)tile.Type == (int)enemy.PlayerColor) || playerIsOverrun ? true : false;
        }

        public void RemoveEnemy(Enemy enemy, Tile tile)
        {
            _enemyRepo.RemoveEnemy(enemy);

            // Check if player is no longer overrun
            var player = _playerRepo.GetPlayerBasedOnColor((Enums.PlayerColor)tile.Type); // get the player color of the tile
            if (_playerLogic.CheckIfPlayerIsOverrun(player))
            {
                _playerRepo.SetPlayerOverrun(player, true);
            }
        }
    }
}