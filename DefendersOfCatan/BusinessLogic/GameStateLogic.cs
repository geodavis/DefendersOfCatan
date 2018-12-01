using DefendersOfCatan.BusinessLogic.Repository;
using System;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.BusinessLogic
{
    public interface IGameStateLogic
    {
        GameState GetCurrentGameState();
        GameState UpdateGameState();
    }
    public class GameStateLogic : IGameStateLogic
    {
        private readonly IBaseRepository _baseRepo;
        public GameStateLogic(IBaseRepository baseRepo)
        {
            _baseRepo = baseRepo;
        }

        public GameState GetCurrentGameState()
        {
            return _baseRepo.GetGame().GameState;
        }
        public GameState UpdateGameState()
        {
            var game = _baseRepo.GetGame();
            var nextGameState = GetNextGameState(game.GameState);
            game.GameState = nextGameState;
            _baseRepo.Save(); // save game state to DB
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