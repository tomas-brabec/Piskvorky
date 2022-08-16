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

        public string PlayerName { get; set; }
        public string OpponentName { get; set; }

        public Player ThisPlayer;
        public Player CurrentPlayer;

        public Game(int boardSize)
        {
            BoardSize = boardSize;
            Board = new int[BoardSize, BoardSize];
            PlayerName = $"Player {Random.Shared.Next(100, 1000)}";
        }
    }

    internal enum Player
    {
        Empty,
        X,
        O,
    }
}
