using System;
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
        public List<Player> GetPlayers()
        {
            var players = playerRepo.GetPlayers();

            return players;
        }

    }
}