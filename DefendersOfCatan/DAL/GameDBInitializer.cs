using DefendersOfCatan.Common;
using DefendersOfCatan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;
using static DefendersOfCatan.Common.Constants;

namespace DefendersOfCatan.DAL
{
    public class GameDBInitializer : System.Data.Entity.DropCreateDatabaseAlways<GameContext>
    {
        protected override void Seed(GameContext context)
        {
            //var students = new List<Student>
            //{
            //new Student{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2005-09-01")},
            //new Student{FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2002-09-01")},
            //new Student{FirstMidName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2003-09-01")},
            //new Student{FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2002-09-01")},
            //new Student{FirstMidName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2002-09-01")},
            //new Student{FirstMidName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2001-09-01")},
            //new Student{FirstMidName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2003-09-01")},
            //new Student{FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2005-09-01")}
            //};

            //students.ForEach(s => context.Students.Add(s));

            //var players = new List<Player>
            //{
            //new Player { Name = "GeoffR", Color = PlayerColor.Red, IsOverrun = false, Health = 5 },
            //new Player { Name = "GeoffB", Color = PlayerColor.Blue, IsOverrun = false, Health = 5 },
            //new Player { Name = "GeoffY", Color = PlayerColor.Yellow, IsOverrun = false, Health = 5  },
            //new Player { Name = "GeoffG", Color = PlayerColor.Green, IsOverrun = false, Health = 5  }
            //};
            
            //players.ForEach(p => context.Players.Add(p));
            //context.SaveChanges();

            //var enemies = new List<Enemy>();
            //var playerColorValues = Enum.GetValues(typeof(PlayerColor));
            //var random = new Random();

            //for (int i = 0; i < NUM_ENEMIES; i++)
            //{
            //    var randomPlayerColor = (PlayerColor)playerColorValues.GetValue(random.Next(playerColorValues.Length));
            //    var enemy = new Enemy { BarbarianIndex = 0, HasBarbarian = random.NextDouble() > 0.5, PlayerColor = randomPlayerColor };
            //    enemies.Add(enemy);
            //}

            //enemies.ForEach(e => context.Enemies.Add(e));
            //context.SaveChanges();
        }
    }
}