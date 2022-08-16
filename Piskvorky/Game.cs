using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    internal class Game
    {
        public int[,] Board { get; init; }
        public int BoardSize { get; init; }

        public Game(int boardSize)
        {
            BoardSize = boardSize;
            Board = new int[BoardSize, BoardSize];
        }
    }

    internal enum Player
    {
        Empty,
        X,
        O,
    }
}
