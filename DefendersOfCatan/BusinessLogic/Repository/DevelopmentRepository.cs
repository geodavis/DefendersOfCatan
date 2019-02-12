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
        void AddCards(List<Card> cards);
        List<Development> GetDevelopments();
        Player GetCurrentPlayerBase();
        Development GetDevelopmentByType(DevelopmentType type);
        List<Card> GetCards();
    }
    public class DevelopmentRepository : BaseRepository, IDevelopmentRepository
    {
        public DevelopmentRepository() { }
        public DevelopmentRepository(IGameContext db) : base(db)
        {

        }

        public void AddDevelopments(List<Development> developments)
        {
            foreach (var development in developments)
            {
                _db.GetSet<Development>().Add(development);
            }
            _db.SaveChanges();
        }

        public void AddCards(List<Card> cards)
        {
            foreach (var card in cards)
            {
                _db.GetSet<Card>().Add(card);
            }
            _db.SaveChanges();
        }

        public List<Development> GetDevelopments()
        {
            return _db.GetSet<Development>().ToList();
        }

        public List<Card> GetCards()
        {
            return _db.GetSet<Card>().ToList();
        }

    }
}