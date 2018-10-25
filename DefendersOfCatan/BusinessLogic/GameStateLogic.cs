using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic
{
    public class GameStateLogic
    {
        private BaseRepository baseRepo = new BaseRepository();

        public GameState GetCurrentGameState()
        {
            return baseRepo.GetGame().GameState;
        }
        public GameState UpdateGameState()
        {
            var game = baseRepo.GetGame();
            var nextGameState = GetNextGameState(game.GameState);
            game.GameState = nextGameState;
            baseRepo.Save(); // save game state to DB
            return game.GameState;
        }

        private GameState GetNextGameState(GameState gameState)
        {
            var currentEnumIndex = (int)gameState;
            var gameStateEnumValues = Enum.GetValues(typeof(GameState));
            return currentEnumIndex == gameStateEnumValues.Length - 1 ? (GameState)gameStateEnumValues.GetValue(2) : (GameState)gameStateEnumValues.GetValue(currentEnumIndex + 1);
        }
    }
}