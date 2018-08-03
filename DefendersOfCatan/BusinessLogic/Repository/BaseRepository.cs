using DefendersOfCatan.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class BaseRepository
    {
        protected readonly GameContext db;

        public BaseRepository()
        {
            db = new GameContext();
        }
    }
}