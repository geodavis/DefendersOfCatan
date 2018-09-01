using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.DAL.DataModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.BusinessLogic
{
    public class ItemLogic
    {
        private ItemRepository itemRepo = new ItemRepository();

        public List<Item> GetItems()
        {
            return itemRepo.GetItems();
        }
    }
}