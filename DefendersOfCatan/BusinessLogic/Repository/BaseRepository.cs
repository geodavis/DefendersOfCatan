using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class BaseRepository
    {
        protected readonly GameContext db;

        public BaseRepository()
        {
            db = new GameContext();
        }

        public Game GetGame()
        {
            return db.Game.Single();
        }

        public Player GetCurrentPlayerBase()
        {
            return GetGame().CurrentPlayer;
        }
        public Development GetDevelopmentByType(DevelopmentType type)
        {
            return db.Developments.Single(i => i.DevelopmentType == type);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}