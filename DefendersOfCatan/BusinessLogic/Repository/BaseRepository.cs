using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    interface IBaseRepository
    {
        void Save();
        void Add(object item);

    }
    public class BaseRepository : IBaseRepository
    {
        protected readonly IGameContext _db;

        public BaseRepository() { }

        public BaseRepository(IGameContext db)
        {
            _db = db;
        }

        public Game GetGame()
        {
            return _db.GetSet<Game>().Single();
        }

        public Player GetCurrentPlayerBase()
        {
            return GetGame().CurrentPlayer;
        }
        public Development GetDevelopmentByType(DevelopmentType type)
        {
            return _db.GetSet<Development>().Single(i => i.DevelopmentType == type);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Add(object item)
        {
            _db.Add(item);
        }
    }
}