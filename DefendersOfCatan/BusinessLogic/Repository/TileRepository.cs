using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class TileRepository : BaseRepository
    {
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

        public void UpdateTile(Tile tile)
        {

        }
    }
}