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
    public interface IPlayerRepository
    {
        List<Player> GetPlayers();
        Player GetCurrentPlayer();
        void RemoveDevelopmentFromCurrentPlayer(DevelopmentType developmentType);
        Player GetPlayerBasedOnColor(PlayerColor playerColor);
        void SetPlayerOverrun(Player player, bool isOverrun);
        bool GetPlayerOverrunBasedOnPlayerColor(PlayerColor playerColor);
        void AddResourceToCurrentPlayer(ResourceType resourceType);
        void RemoveResourceFromCurrentPlayer(ResourceType resourceType, int qty);
        void AddDevelopmentToCurrentPlayer(DevelopmentType developmentType);
        void AddCardToCurrentPlayer(CardType cardType);
        void SetCanMoveToAnyTile(bool canMoveToAnyTile);

    }
    public class PlayerRepository : BaseRepository, IPlayerRepository
    {
        public PlayerRepository() { }
        public PlayerRepository(IGameContext db) : base(db)
        {

        }
        public List<Player> GetPlayers()
        {
            var players = _db.GetSet<Game>().FirstOrDefault().Players;
            return players;
        }

        public Player GetCurrentPlayer()
        {
            return GetCurrentPlayerBase();
        }

        public Player GetPlayerBasedOnColor(PlayerColor playerColor)
        {
            return GetGame().Players.Single(p => (int)p.Color == (int)playerColor);
        }

        public void SetPlayerOverrun(Player player, bool isOverrun)
        {
            player.IsOverrun = isOverrun;
            _db.SaveChanges();
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
            var playerResource = currentPlayer.PlayerResources.Single(r => r.ResourceType == resourceType);
            playerResource.Qty += 1;
            _db.SaveChanges();
        }

        public void RemoveResourceFromCurrentPlayer(ResourceType resourceType, int qty)
        {
            var playerResource = GetCurrentPlayer().PlayerResources.Single(r => r.ResourceType == resourceType);
            playerResource.Qty -= qty;
            _db.SaveChanges();
        }

        public void AddDevelopmentToCurrentPlayer(DevelopmentType developmentType)
        {
            var player = GetCurrentPlayer();
            var playerDevelopment = player.PlayerDevelopments.Single(i => i.DevelopmentType == developmentType);
            playerDevelopment.Qty += 1;
            player.HasPurchasedItems = true;
            _db.SaveChanges();
        }

        public void AddCardToCurrentPlayer(CardType cardType)
        {
            var player = GetCurrentPlayer();
            var playerCard = player.PlayerCards.Single(i => i.CardType == cardType);
            playerCard.Qty += 1;
            _db.SaveChanges();
        }

        public void RemoveDevelopmentFromCurrentPlayer(DevelopmentType developmentType)
        {
            var player = GetCurrentPlayer();
            var playerDevelopment = player.PlayerDevelopments.Single(i => i.DevelopmentType == developmentType);
            playerDevelopment.Qty -= 1;
            player.HasPurchasedItems = false;
            _db.SaveChanges();
        }

        public void SetCanMoveToAnyTile(bool canMoveToAnyTile)
        {
            var player = GetCurrentPlayer();
            player.CanMoveToAnyTile = canMoveToAnyTile;
            _db.SaveChanges();
        }
    }
}