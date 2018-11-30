using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public interface IGameRepository
    {
        void Save();
        void AddGame(Game game);
    }
    public class GameRepository : BaseRepository, IGameRepository
    {
        public GameRepository(IGameContext db) : base(db)
        {
            
        }

        public void AddGame(Game game)
        {
            _db.GetSet<Game>().Add(game);
            _db.SaveChanges();
        }

    }
}