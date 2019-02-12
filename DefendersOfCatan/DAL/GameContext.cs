using DefendersOfCatan.DAL.DataModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DefendersOfCatan.DAL
{
    public interface IGameContext
    {
        DbSet<T> GetSet<T>() where T : class;
        void Add<T>(T item) where T : class;
        int SaveChanges();
    }

    public class GameContext : DbContext, IGameContext
    {
        public GameContext() : base("GameContext") { }

        public DbSet<Game> Game { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<Enemy> Enemies { get; set; }

        public DbSet<Tile> Tiles { get; set; }

        public DbSet<Development> Developments { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Road> Roads { get; set; }

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