using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.BusinessLogic
{
    public class DevelopmentLogic
    {
        private DevelopmentRepository developmentRepo = new DevelopmentRepository();

        public List<DevelopmentTransfer> GetDevelopments()
        {
            var developmentsTransfer = new List<DevelopmentTransfer>();
            var developments = developmentRepo.GetDevelopments();

            foreach (var development in developments)
            {
                var developmentTransfer = new DevelopmentTransfer();
                developmentTransfer.DevelopmentType = development.DevelopmentType;
                developmentTransfer.DevelopmentCost = development.DevelopmentCost;
                developmentTransfer.DevelopmentTypeReadable = development.DevelopmentType.ToString();
                developmentsTransfer.Add(developmentTransfer);
            }
            return developmentsTransfer;
        }

        public bool PlacePurchasedDevelopment()
        {
            var hasDevelopmentToPlace = false;
            var currentPlayerDevelopmentsWithQty = developmentRepo.GetCurrentPlayerBase().PlayerDevelopments.Where(i => i.Qty > 0).Single();
            if (currentPlayerDevelopmentsWithQty != null)
            {
                // ToDo: Add building to Tile
                hasDevelopmentToPlace = true;
            }

            return hasDevelopmentToPlace;
        }
    }
}