﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.DAL;
using DefendersOfCatan.BusinessLogic.Repository;

namespace DefendersOfCatan.BusinessLogic
{
    public class PlayerLogic
    {
        private PlayerRepository playerRepo = new PlayerRepository();
        private TileRepository tileRepo = new TileRepository();
        private TileLogic tileLogic = new TileLogic();

        public List<Player> GetPlayers()
        {
            return playerRepo.GetPlayers();
        }

        public Player GetCurrentPlayer()
        {
            return playerRepo.GetCurrentPlayer();
        }

        public bool MovePlayerToTile(int selectedTileId)
        {
            var currentPlayerTile = tileRepo.GetCurrentPlayerTile();
            var selectedTile = tileRepo.GetTileById(selectedTileId);
            var neighborTiles = tileLogic.GetNeighborTiles(currentPlayerTile);

            if (neighborTiles.Any(t => t.Id == selectedTile.Id) || selectedTile.Id == currentPlayerTile.Id)
            {
                selectedTile.Players.Add(GetCurrentPlayer());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}