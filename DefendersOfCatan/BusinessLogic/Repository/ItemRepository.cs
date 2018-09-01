using DefendersOfCatan.DAL.DataModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class ItemRepository : BaseRepository
    {
        public Item GetItemByType(int type)
        {
            var items = db.Items;

            return new Item();// db.Items.Where(i => i.GameItems.Where(i => i.ItemType == type));
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