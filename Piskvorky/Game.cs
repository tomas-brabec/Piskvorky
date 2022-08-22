using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    public class Game
    {
        public int[,] Board { get; init; }
        public int BoardSize { get; init; }

        public string PlayerName { get; set; }
        public Player PlayerMark { get; set; }
        public Player OpponentMark => PlayerMark == Player.X ? Player.O : Player.X;
        public Player CurrentPlayer { get; set; }

        public bool IsRunning { get; set; }
        public bool Winner { get; set; }

        public (int x, int y) First { get; set; }
        public (int x, int y) Last { get; set; }

        public Game(int boardSize)
        {
            BoardSize = boardSize;
            Board = new int[BoardSize, BoardSize];
            PlayerName = $"Player {Random.Shared.Next(100, 1000)}";

            CurrentPlayer = Player.X;
        }

        public void Reset()
        {
            for (int i = 0; i < Board.GetLength(0); i++)
                for (int j = 0; j < Board.GetLength(1); j++)
                    Board[i, j] = 0;

            Winner = false;
            IsRunning = false;
        }

        public bool NextMove(int x, int y)
        {
            if (Board[x, y] == (int)Player.Empty)
            {
                Board[x, y] = (int)CurrentPlayer;
                FindRow(x, y);
                return true;
            }

            return false;
        }

        private void FindRow(int startX, int startY)
        {
            var vectorsToCheck = new int[,] { { 1, 0 }, { 0, 1 }, { 1, -1 }, { 1, 1 } };
            var rowSize = 2;
            (int x, int y) first = (startX, startY);
            (int x, int y) last = (startX, startY);

            for (int i = 0; i < vectorsToCheck.GetLength(0); i++)
            {
                var forward = true;
                var backward = true;
                var count = 1;

                for (int j = 1; j <= rowSize; j++)
                {
                    if (forward)
                    {
                        var nextX = startX + vectorsToCheck[i, 0] * j;
                        var nextY = startY + vectorsToCheck[i, 1] * j;
                        if (IsInRange(nextX, nextY))
                        {
                            if (Board[nextX, nextY] == (int)CurrentPlayer)
                            {
                                last = (nextX, nextY);
                                count++;
                            }
                            else
                            {
                                forward = false;
                            }
                        }
                        else
                        {
                            forward = false;
                        }
                    }

                    if (backward)
                    {
                        var nextX = startX + vectorsToCheck[i, 0] * j * -1;
                        var nextY = startY + vectorsToCheck[i, 1] * j * -1;
                        if (IsInRange(nextX, nextY))
                        {
                            if (Board[nextX, nextY] == (int)CurrentPlayer)
                            {
                                first = (nextX, nextY);
                                count++;
                            }
                            else
                            {
                                backward = false;
                            }
                        }
                        else
                        {
                            backward = false;
                        }
                    }

                    if (count >= rowSize)
                    {
                        Winner = true;
                        First = first;
                        Last = last;
                        return;
                    }
                }
            }
        }

        private bool IsInRange(int x, int y)
        {
            if (x >= 0 && x < BoardSize && y >= 0 && y < BoardSize)
                return true;
            else
                return false;
        }
    }

    public enum Player
    {
        Empty,
        X,
        O,
    }
}
