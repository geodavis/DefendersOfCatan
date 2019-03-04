using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.Common;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic
{
    public interface IEnemyLogic
    {
        ClickedTileTransfer AddEnemyToTile(ClickedTileTransfer tileTransfer);
        EnemyFightTransfer RemoveEnemy(int enemyId);
        EnemyMoveTransfer ExecuteEnemyMovePhase();
        bool IsEnemyTileNeighbor(int enemyId);
        List<int> GetNeighborEnemyTileIds();
        List<int> RollDice();
        List<Tile> PushBarbarianBack(TileType tileType);
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
            var enemyMoveTransfer = new EnemyMoveTransfer { BarbarianTiles = new List<Tile>(), OverrunTiles = new List<Tile>(), OverrunDevelopments = new List<OverrunTileDevelopment>() };
            var tiles = new List<Tile>();

            // Check current player for barbarian advancement
            foreach (var tile in _tileRepo.GetTiles())
            {
                if ((int)_playerRepo.GetCurrentPlayer().Color == (int)tile.Type && tile.Enemy != null && tile.Enemy.HasBarbarian)
                {
                    var barbarianIndex = tile.Enemy.BarbarianIndex + 1;
                    var barbarianStrength = tile.Enemy.Strength;
                    // Reset barbarian index if it hits 3, and overrun the appropriate tile
                    if (barbarianIndex == 3)
                    {
                        var overrunTile = _tileLogic.GetOverrunTile(tile);
                        if (!_tileLogic.TileHasSettlement(overrunTile.Id))
                        {
                            _tileRepo.SetOverrunTile(overrunTile);
                            enemyMoveTransfer.OverrunTiles.Add(overrunTile); // Add tile to overrun
                        }
                        else
                        {
                            _tileRepo.RemoveDevelopmentFromTile(overrunTile.Id, DevelopmentType.Settlement);
                            enemyMoveTransfer.OverrunDevelopments.Add(new OverrunTileDevelopment { DevelopmentType = DevelopmentType.Settlement, Tile = overrunTile }); // Add development to remove from tile
                        }

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

        public ClickedTileTransfer AddEnemyToTile(ClickedTileTransfer tileTransfer)
        {
            var transfer = new ClickedTileTransfer();
            var tile = _tileRepo.GetTileById(tileTransfer.ClickedTileId);
            var enemy = _enemyRepo.GetSelectedEnemy();
            transfer.EnemyId = enemy.Id;
            if (CanEnemyBeAddedToTile(enemy, tile))
            {
                _tileRepo.AddEnemyToTile(enemy.Id, tile.Id);
                _enemyRepo.SetEnemyPlaced(enemy);
                var player = _playerRepo.GetPlayerBasedOnColor((PlayerColor)tile.Type);
                transfer.PlayerId = player.Id;
                if (_playerLogic.CheckIfPlayerIsOverrun(player))
                {
                    _playerRepo.SetPlayerOverrun(player, true);
                    transfer.IsOverrun = true;
                }
            }
            else
            {
                // ToDo: return error if validation fails
            }

            return transfer;

        }
        private bool CanEnemyBeAddedToTile(Enemy enemy, Tile tile)
        {
            var playerIsOverrun = _playerRepo.GetPlayerOverrunBasedOnPlayerColor(enemy.PlayerColor);
            return tile.IsEnemyTile() && ((int)tile.Type == (int)enemy.PlayerColor) || playerIsOverrun ? true : false;
        }
        public bool IsEnemyTileNeighbor(int enemyId)
        {
            var currentPlayerTile = _tileRepo.GetCurrentPlayerTile();
            var enemyTile = _tileRepo.GetTiles().Single(t => t.Enemy != null && t.Enemy.Id == enemyId);
            var neighborTiles = _tileLogic.GetNeighborTiles(currentPlayerTile);

            if (neighborTiles.Contains(enemyTile))
            {
                return true;
            }

            return false;
        }
        public List<int> GetNeighborEnemyTileIds()
        {
            var neighborEnemyTileIds = new List<int>();
            var currentPlayerTile = _tileRepo.GetCurrentPlayerTile();
            var enemyTiles = _tileRepo.GetTilesWithEnemyIds();
            var neighborTiles = _tileLogic.GetNeighborTiles(currentPlayerTile);

            foreach (var tile in neighborTiles)
            {
                if (enemyTiles.Contains(tile.Id))
                {
                    neighborEnemyTileIds.Add(tile.Id);
                }
            }

            return neighborEnemyTileIds;
        }
        public List<int> RollDice()
        {
            var diceRolls = new List<int>();
            // Roll dice to see if enemy can be removed
            var rnd = new Random();
            var dice = rnd.Next(1, 7);   // creates a number between 1 and 6
            diceRolls.Add(dice);

            return diceRolls;
        }

        public EnemyFightTransfer RemoveEnemy(int enemyId)
        {
            var enemyFightTransfer = new EnemyFightTransfer();
            var enemyTile = _tileRepo.GetTiles().Single(t => t.Enemy != null && t.Enemy.Id == enemyId);
            enemyFightTransfer.EnemyTile = enemyTile;
            _enemyRepo.RemoveEnemy(enemyId);
            // Check if player is no longer overrun
            var player = _playerRepo.GetPlayerBasedOnColor((PlayerColor)enemyTile.Type); // get the player color of the enemy tile
            enemyFightTransfer.OverrunPlayerId = player.Id;
            if (_playerLogic.CheckIfPlayerIsOverrun(player))
            {
                _playerRepo.SetPlayerOverrun(player, false);
            }

            return enemyFightTransfer;
        }

        public List<Tile> PushBarbarianBack(TileType tileType)
        {
            var tiles = _tileRepo.GetTiles().Where(t => t.Type == tileType).ToList();
            var barbarianTiles = new List<Tile>();

            foreach (var tile in tiles)
            {
                if (tile.Enemy != null && tile.Enemy.HasBarbarian)
                {
                    var barbarianIndex = tile.Enemy.BarbarianIndex == 0 ? 0 : tile.Enemy.BarbarianIndex - 1;
                    _enemyRepo.UpdateBarbarian(tile.Enemy, barbarianIndex, tile.Enemy.Strength);
                    barbarianTiles.Add(tile);
                }
            }

            return barbarianTiles;
        }
    }
}