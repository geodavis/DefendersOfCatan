using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class TileRepository : BaseRepository
    {
        private DevelopmentRepository developmentRepo = new DevelopmentRepository();

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
            return GetGame().Tiles.Where(t => t.Id == tileId).Single();
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
            var tileDevelopment = new TileDevelopment();
            tileDevelopment.Development = development;
            tile.Developments.Add(tileDevelopment);
            db.SaveChanges();
        }

        public List<TileDevelopment> GetDevelopments(int tileId)
        {
            var tile = GetTileById(tileId);
            return tile.Developments;
        }
    }
}