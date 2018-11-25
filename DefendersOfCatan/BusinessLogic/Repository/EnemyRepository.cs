using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefendersOfCatan.DAL;

namespace DefendersOfCatan.BusinessLogic.Repository
{
    public class EnemyRepository : BaseRepository
    {

        public List<Enemy> GetEnemies()
        {
            return GetGame().Enemies;
        }

        public Enemy GetEnemy(int id)
        {
            return GetGame().Enemies.Single(e => e.Id == id);
        }

        public Enemy GetSelectedEnemy()
        {
            return GetGame().Enemies.Single(e => e.IsSelected);
        }

        public void SetEnemyPlaced(Enemy enemy)
        {
            enemy.IsSelected = false;
            enemy.HasBeenPlaced = true;
            db.SaveChanges();
        }

        public void UpdateEnemy(UpdateEnemyTransfer enemyTransfer)
        {
            var enemy = GetEnemy(enemyTransfer.Id);
            enemy.HasBeenPlaced = enemyTransfer.HasBeenPlaced;
            enemy.BarbarianIndex = enemyTransfer.BarbarianIndex;
            db.SaveChanges();
        }

        public void UpdateBarbarianIndex(Enemy enemy, int barbarianIndex)
        {
            enemy.BarbarianIndex = barbarianIndex;
            db.SaveChanges();
        }

        public void RemoveEnemy(Enemy enemy)
        {
            enemy.IsRemoved = true;
            db.SaveChanges();
        }
    }
}