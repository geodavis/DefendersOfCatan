using DefendersOfCatan.Common;
using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class PlayerRepository : BaseRepository
    {
        public List<Player> GetPlayers()
        {
            var players = db.Game.FirstOrDefault().Players;
            return players;
        }

        public Player GetCurrentPlayer()
        {
            return GetCurrentPlayerBase();
        }

        public Player GetPlayerBasedOnColor(Enums.PlayerColor playerColor)
        {
            return GetGame().Players.Where(p => (int)p.Color == (int)playerColor).Single();
        }

        public void SetPlayerOverrun(Player player, bool isOverrun)
        {
            player.IsOverrun = isOverrun;
            db.SaveChanges();
        }

        public bool GetPlayerOverrunBasedOnPlayerColor(Enums.PlayerColor playerColor)
        {
            var playerToCheck = GetPlayerBasedOnColor(playerColor);
            if (playerToCheck.IsOverrun)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}