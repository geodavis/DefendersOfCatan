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
            var players = db.GetSet<Game>().FirstOrDefault().Players;
            return players;
        }

    }
}