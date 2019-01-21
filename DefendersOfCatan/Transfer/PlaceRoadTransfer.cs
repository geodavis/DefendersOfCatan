using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.Transfer
{
    public class PlaceRoadTransfer
    {
        public int Tile1Id { get; set; }
        public int Tile2Id { get; set; }
        public int Angle { get; set; }
        public string GameState { get; set; }
        public int DevelopmentType { get; set; }
    }
}