
using DefendersOfCatan.Models;
using static DefendersOfCatan.Common.Enums;

namespace DefendersOfCatan.Common
{
    public class Globals
    {
        private Globals() { } // ToDo: Continue to develop singleton https://www.c-sharpcorner.com/article/singleton/

        public static readonly int[,] TileLayout = new int[,]
        {
            {7, 7, 0, 0, 1, 1, 7, 7},
            {7, 0, 4, 4, 4, 1, 7, 7},
            {7, 6, 4, 4, 4, 4, 6, 7},
            {6, 4, 4, 5, 4, 4, 6, 7},
            {7, 6, 4, 4, 4, 4, 6, 7},
            {7, 3, 4, 4, 4, 2, 7, 7},
            {7, 7, 3, 3, 2, 2, 7, 7}
        };

        public static readonly string[,] HexOverrunData = new string[,]
        {
            {"0", "0", "36", "37", "46", "47", "0", "0"},
            {"0", "35", "36,54", "37,46,53,66", "47,65", "48", "0", "0"},
            {"0", "7", "35,55", "36,46,54,66", "37,47,53,65", "48,64", "7", "0"},
            {"7", "1", "35,46,55,66", "36,47,54,65", "37,48,53,64", "1", "7", "0"},
            {"0", "7", "46,66", "35,47,55,65", "36,48,54,64", "37,53", "7", "0"},
            {"0", "66", "47,65", "35,48,55,64", "36,54", "53", "0", "0"},
            {"0", "0", "65", "64", "55", "54", "0", "0"}
        };

        public static GameState GameState;

    }
}