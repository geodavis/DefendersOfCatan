using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.DAL.DataModels.Items;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DefendersOfCatan.DAL
{
    public class GameContext : DbContext
    {
        public GameContext() : base("GameContext")
        {

        }
        public DbSet<Game> Game { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<Enemy> Enemies { get; set; }

        public DbSet<Tile> Tiles { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<PlayerResource> PlayerResources { get; set; }


        public DbSet<T> GetSet<T>() where T : class
        {
            return Set<T>();
        }
        public void Add<T>(T item) where T : class
        {
            Set<T>().Add(item);
        }
        public int SqlCommand(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(sql, parameters);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}