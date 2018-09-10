using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class DevelopmentRepository : BaseRepository
    {
        public Development GetDevelopmentByType(DevelopmentType type)
        {
            return db.Developments.Where(i => i.DevelopmentType == type).Single();
        }

        public void AddDevelopments(List<Development> developments)
        {
            foreach (var development in developments)
            {
                db.Developments.Add(development);
            }
            db.SaveChanges();
        }

        public List<Development> GetDevelopments()
        {
            return db.Developments.ToList();
        }

    }
}