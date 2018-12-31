using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefendersOfCatan.DAL;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public interface IEnemyRepository
    {
        List<Enemy> GetEnemies();
        Enemy GetEnemy(int id);
        Enemy GetSelectedEnemy();
        void UpdateEnemy(UpdateEnemyTransfer enemyTransfer);
        Game GetGame();
        void UpdateBarbarian(Enemy enemy, int barbarianIndex, int strength);
        void SetEnemyPlaced(Enemy enemy);
        void RemoveEnemy(Enemy enemy);
        bool SetSelectedEnemy(int id);
    }
    public class EnemyRepository : BaseRepository, IEnemyRepository
    {
        public EnemyRepository() { }

        public EnemyRepository(IGameContext db) : base(db)
        {

        }


        public List<Enemy> GetEnemies()
        {
            return GetGame().Enemies;
        }

        public Enemy GetEnemy(int id)
        {
            return GetGame().Enemies.Single(e => e.Id == id);
        }

        public bool SetSelectedEnemy(int id)
        {
            // Validate a selected enemy does not already exist
            if (GetSelectedEnemy() == null) // ToDo: Return error to client
            {
                var enemy = GetGame().Enemies.Single(e => e.Id == id);
                enemy.IsSelected = true;
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public Enemy GetSelectedEnemy()
        {
            return GetGame().Enemies.FirstOrDefault(e => e.IsSelected);
        }

        public void SetEnemyPlaced(Enemy enemy)
        {
            enemy.IsSelected = false;
            enemy.HasBeenPlaced = true;
            _db.SaveChanges();
        }

        public void UpdateEnemy(UpdateEnemyTransfer enemyTransfer)
        {
            var enemy = GetEnemy(enemyTransfer.Id);
            enemy.HasBeenPlaced = enemyTransfer.HasBeenPlaced;
            enemy.BarbarianIndex = enemyTransfer.BarbarianIndex;
            _db.SaveChanges();
        }

        public void UpdateBarbarian(Enemy enemy, int barbarianIndex, int strength)
        {
            enemy.BarbarianIndex = barbarianIndex;
            enemy.Strength = strength;
            _db.SaveChanges();
        }

        public void RemoveEnemy(Enemy enemy)
        {
            enemy.IsRemoved = true;
            _db.SaveChanges();
        }
    }
}