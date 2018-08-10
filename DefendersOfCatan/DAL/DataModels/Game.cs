using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.DAL.DataModels
{
    public class Game
    {
        public int Id { get; set; }
        public virtual List<Tile> Tiles { get; set; }
        public virtual List<Player> Players { get; set; }
        public virtual List<Enemy> Enemies { get; set; }
        public virtual Player CurrentPlayer { get; set; }
        public virtual GameState GameState { get; set; }

    }
}