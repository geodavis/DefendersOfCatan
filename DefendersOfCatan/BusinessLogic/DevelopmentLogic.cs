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
    public interface IDevelopmentLogic
    {
        List<DevelopmentTransfer> GetDevelopments();
        DevelopmentType PlaceInitialSettlement(int tileId);
        DevelopmentType PlacePurchasedDevelopment(int tileId);

    }
    public class DevelopmentLogic : IDevelopmentLogic
    {
        private readonly IDevelopmentRepository _developmentRepo;
        private readonly ITileRepository _tileRepo;
        private readonly IPlayerRepository _playerRepo;
        private readonly ITileLogic _tileLogic;
        public DevelopmentLogic(IDevelopmentRepository developmentRepo, ITileRepository tileRepo, IPlayerRepository playerRepo, ITileLogic tileLogic)
        {
            _developmentRepo = developmentRepo;
            _tileRepo = tileRepo;
            _playerRepo = playerRepo;
            _tileLogic = tileLogic;

        }

        public List<DevelopmentTransfer> GetDevelopments()
        {
            var developmentsTransfer = new List<DevelopmentTransfer>();
            var developments = _developmentRepo.GetDevelopments();

            foreach (var development in developments)
            {
                var developmentTransfer = new DevelopmentTransfer
                {
                    DevelopmentType = development.DevelopmentType,
                    DevelopmentCost = development.DevelopmentCost,
                    DevelopmentTypeReadable = development.DevelopmentType.ToString()
                };
                developmentsTransfer.Add(developmentTransfer);
            }
            return developmentsTransfer;
        }

        public DevelopmentType PlacePurchasedDevelopment(int tileId)
        {
            var currentPlayerDevelopmentsWithQty = _developmentRepo.GetCurrentPlayerBase().PlayerDevelopments.Single(i => i.Qty > 0 && i.DevelopmentType != DevelopmentType.Card);
            var developmentType = currentPlayerDevelopmentsWithQty.DevelopmentType;
            if (currentPlayerDevelopmentsWithQty != null)
            {
                switch (developmentType)
                {
                    case DevelopmentType.Road:
                        // ToDo:
                        // Get adjacent tile, based on angle of selected placeable
                        _tileLogic.GetAdjacentTileBasedOnAngle(tileId, 0);

                        // Tile repo - add road to both selected and adjoining tile

                        break;
                    case DevelopmentType.Settlement:
                        _tileRepo.AddDevelopmentToTile(tileId, developmentType);
                        break;
                    case DevelopmentType.City:
                        break;
                    case DevelopmentType.Walls:
                        break;
                    case DevelopmentType.Card:
                        break;
                    default:
                        break;
                }

            }

            // Remove development from player
            _playerRepo.RemoveDevelopmentFromCurrentPlayer(developmentType);

            return developmentType;
        }

        public DevelopmentType PlaceInitialSettlement(int tileId)
        {
            var developmentType = DevelopmentType.Settlement;
            _tileRepo.AddDevelopmentToTile(tileId, developmentType);
            return developmentType;
        }

        public void RemoveSettlement(int tileId)
        {
            var developmentType = DevelopmentType.Settlement;
            _tileRepo.RemoveDevelopmentFromTile(tileId, developmentType);
        }
    }
}