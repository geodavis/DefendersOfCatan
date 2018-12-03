using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;
using DefendersOfCatan.DAL;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public interface ITileRepository
    {
        List<Tile> GetTiles();
        void AddDevelopmentToTile(int tileId, DevelopmentType developmentType);
        void RemoveDevelopmentFromTile(int tileId, DevelopmentType developmentType);
        Tile GetTileById(int tileId);
        void AddEnemyToTile(int enemyId, int tileId);
        Tile GetCurrentPlayerTile();
        void UpdateCurrentPlayerTile(Tile tile);
        void SetOverrunTile(Tile tile);
        List<TileDevelopment> GetDevelopments(int tileId);
        List<Tile> GetPlaceableTiles(DevelopmentType developmentType);
    }
    public class TileRepository : BaseRepository, ITileRepository
    {
        private readonly IDevelopmentRepository _developmentRepo;
        private readonly IEnemyRepository _enemyRepo;

        public TileRepository() { }
        public TileRepository(IGameContext db, IDevelopmentRepository developmentRepo, IEnemyRepository enemyRepo) : base(db)
        {
            _developmentRepo = developmentRepo;
            _enemyRepo = enemyRepo;
        }

        public Tile GetCurrentPlayerTile()
        {
            var game = GetGame();
            var currentPlayer = GetCurrentPlayerBase();
            return game.Tiles.Single(t => t.Players.Any(p => p.Id == currentPlayer.Id));
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
            _db.SaveChanges();
        }

        public void UpdateCurrentPlayerTile(Tile tile)
        {
            tile.Players.Add(GetCurrentPlayerBase()); // (For future reference) Passing player to this method causes issues with EF. It creates a new record, rather than updating an existing record. This is caused by passing by reference.
            _db.SaveChanges();
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
            _db.SaveChanges();
        }

        public void RemoveDevelopmentFromTile(int tileId, DevelopmentType developmentType)
        {
            var tile = GetTileById(tileId);
            tile.Developments.RemoveAll(d => d.Development.DevelopmentType == developmentType);
            _db.SaveChanges();
        }

        public List<TileDevelopment> GetDevelopments(int tileId)
        {
            var tile = GetTileById(tileId);
            return tile.Developments;
        }

        public void AddEnemyToTile(int enemyId, int tileId)
        {
            var tile = GetTileById(tileId);
            tile.Enemy = _enemyRepo.GetEnemy(enemyId);
            _db.SaveChanges();
        }

        public List<Tile> GetPlaceableTiles(DevelopmentType developmentType)
        {
            return GetGame().Tiles.Where(t => !t.Developments.Any() && t.Type == TileType.Resource).ToList();
        }
    }
}