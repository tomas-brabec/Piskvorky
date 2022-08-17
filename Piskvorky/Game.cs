using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    internal class Game
    {
        public int[,] Board { get; init; }
        public int BoardSize { get; init; }

        public string PlayerName { get; set; }
        public Player PlayerMark { get; set; }
        public Player CurrentPlayer { get; set; }

        public bool IsRunning { get; set; }

        public Game(int boardSize)
        {
            BoardSize = boardSize;
            Board = new int[BoardSize, BoardSize];
            PlayerName = $"Player {Random.Shared.Next(100, 1000)}";
        }

        public void Reset()
        {
            for (int i = 0; i < Board.GetLength(0); i++)
                for (int j = 0; j < Board.GetLength(1); j++)
                    Board[i, j] = 0;
        }
    }

    internal enum Player
    {
        Empty,
        X,
        O,
    }
}
