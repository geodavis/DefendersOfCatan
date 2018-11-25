using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;
using DefendersOfCatan.DAL;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class TileRepository : BaseRepository
    {
        private DevelopmentRepository developmentRepo = new DevelopmentRepository();
        private EnemyRepository enemyRepo = new EnemyRepository();

        public Tile GetCurrentPlayerTile()
        {
            var game = GetGame();
            var currentPlayer = GetCurrentPlayerBase();
            return game.Tiles.Where(t => t.Players.Any(p => p.Id == currentPlayer.Id)).Single();
        }

        public List<Tile> GetTiles()
        {
            return GetGame().Tiles;
        }

        public Tile GetTileById(int tileId)
        {
            return GetGame().Tiles.Single(t => t.Id == tileId);
        }

        public void SetOverrunTile(Tile tile)
        {
            tile.IsOverrun = true;
            db.SaveChanges();
        }

        public void UpdateCurrentPlayerTile(Tile tile)
        {
            tile.Players.Add(GetCurrentPlayerBase()); // (For future reference) Passing player to this method causes issues with EF. It creates a new record, rather than updating an existing record. This is caused by passing by reference.
            db.SaveChanges();
        }

        public void AddDevelopmentToTile(int tileId, DevelopmentType developmentType)
        {
            var development = GetDevelopmentByType(developmentType);
            var tile = GetTileById(tileId);
            var tileDevelopment = new TileDevelopment
            {
                Development = development
            };
            tile.Developments.Add(tileDevelopment);
            db.SaveChanges();
        }

        public void RemoveDevelopmentFromTile(int tileId, DevelopmentType developmentType)
        {
            var development = GetDevelopmentByType(developmentType);
            var tile = GetTileById(tileId);
            var tileDevelopment = new TileDevelopment { Development = development };
            tile.Developments.Remove(tileDevelopment);
            db.SaveChanges();
        }

        public List<TileDevelopment> GetDevelopments(int tileId)
        {
            var tile = GetTileById(tileId);
            return tile.Developments;
        }

        public void AddEnemyToTile(int enemyId, int tileId)
        {
            //var enemy = enemyRepo.GetEnemy(enemyId);
            var tile = GetTileById(tileId);
            tile.Enemy = enemyRepo.GetEnemy(enemyId);
            //db.Entry(tile.Enemy).State = System.Data.Entity.EntityState.Modified; // ToDo: make single data context that all repos use
            db.SaveChanges();
        }
    }
}