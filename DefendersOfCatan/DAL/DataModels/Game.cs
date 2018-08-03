﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefendersOfCatan.DAL.DataModels
{
    public class Game
    {
        public int Id { get; set; }
        public virtual List<Tile> Tiles { get; set; }
        public virtual List<Player> Players { get; set; }
        public virtual List<Enemy> Enemies { get; set; }
        public virtual Player CurrentPlayer { get; set; }

    }
}