using DefendersOfCatan.DAL.DataModels;
using DefendersOfCatan.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            return GetGame().Enemies.Where(e => e.Id == id).Single();
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
    }
}