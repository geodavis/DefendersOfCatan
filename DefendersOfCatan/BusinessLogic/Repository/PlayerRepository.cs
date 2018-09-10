using DefendersOfCatan.Common;
using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

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

        public Player GetPlayerBasedOnColor(PlayerColor playerColor)
        {
            return GetGame().Players.Where(p => (int)p.Color == (int)playerColor).Single();
        }

        public void SetPlayerOverrun(Player player, bool isOverrun)
        {
            player.IsOverrun = isOverrun;
            db.SaveChanges();
        }

        public bool GetPlayerOverrunBasedOnPlayerColor(PlayerColor playerColor)
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

        public void AddResourceToCurrentPlayer(ResourceType resourceType)
        {
            var currentPlayer = GetCurrentPlayer();
            var playerResource = currentPlayer.PlayerResources.Where(r => r.ResourceType == resourceType).Single();
            playerResource.Qty += 1;
            db.SaveChanges();
        }

        public void RemoveResourceFromCurrentPlayer(ResourceType resourceType, int qty)
        {
            var playerResource = GetCurrentPlayer().PlayerResources.Where(r => r.ResourceType == resourceType).Single();
            playerResource.Qty -= qty;
            db.SaveChanges();
        }

        public void AddDevelopmentToCurrentPlayer(DevelopmentType developmentType)
        {
            var playerDevelopment = GetCurrentPlayer().PlayerDevelopments.Where(i => i.DevelopmentType == developmentType).Single();
            playerDevelopment.Qty += 1;
            db.SaveChanges();
        }
    }
}