using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic
{
    public class DevelopmentLogic
    {
        private DevelopmentRepository developmentRepo = new DevelopmentRepository();
        private TileRepository tileRepo = new TileRepository();

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

        public DevelopmentType PlacePurchasedDevelopment(int tileId)
        {
            var currentPlayerDevelopmentsWithQty = developmentRepo.GetCurrentPlayerBase().PlayerDevelopments.Where(i => i.Qty > 0 && i.DevelopmentType != DevelopmentType.Card).Single();
            var developmentType = currentPlayerDevelopmentsWithQty.DevelopmentType;
            if (currentPlayerDevelopmentsWithQty != null)
            {
                tileRepo.AddDevelopmentToTile(tileId, developmentType);
            }

            return developmentType;
        }

        public DevelopmentType PlaceInitialSettlement(int tileId)
        {
            var developmentType = DevelopmentType.Settlement;
            tileRepo.AddDevelopmentToTile(tileId, developmentType);
            return developmentType;
        }
    }
}