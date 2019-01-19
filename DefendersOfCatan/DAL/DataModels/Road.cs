using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.DAL.DataModels
{
    public class Road
    {
        public int Id { get; set; }
        public virtual Tile Tile1 { get; set; }
        public virtual Tile Tile2 { get; set; }
        public bool Placed { get; set; }
        public virtual int Angle { get; set; }
    }
}