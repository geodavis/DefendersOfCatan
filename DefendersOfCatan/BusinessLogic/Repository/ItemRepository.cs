using DefendersOfCatan.DAL.DataModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class ItemRepository : BaseRepository
    {
        public Item GetItemByType(ItemType type)
        {
            var items = db.Items;
            return db.Items.Where(i => i.ItemType == type).Single();
        }

        public void AddItems(List<Item> items)
        {
            foreach (var item in items)
            {
                db.Items.Add(item);
            }
            db.SaveChanges();
        }

        public List<Item> GetItems()
        {
            return db.Items.ToList();
        }

    }
}