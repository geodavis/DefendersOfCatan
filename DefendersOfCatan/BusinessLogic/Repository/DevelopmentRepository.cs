using DefendersOfCatan.DAL;
using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public interface IDevelopmentRepository
    {
        void AddDevelopments(List<Development> developments);
    }
    public class DevelopmentRepository : BaseRepository, IDevelopmentRepository
    {
        public DevelopmentRepository() { }
        public DevelopmentRepository(IGameContext db) : base(db)
        {

        }
        //public Development GetDevelopmentByType(DevelopmentType type)
        //{
        //    return db.Developments.Where(i => i.DevelopmentType == type).Single();
        //}

        public void AddDevelopments(List<Development> developments)
        {
            foreach (var development in developments)
            {
                _db.GetSet<Development>().Add(development);
            }
            _db.SaveChanges();
        }

        public List<Development> GetDevelopments()
        {
            return _db.GetSet<Development>().ToList();
        }

    }
}