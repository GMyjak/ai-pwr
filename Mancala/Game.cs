using System;
using System.Collections.Generic;
using System.Linq;

namespace Mancala
{
    public struct Move
    {
        public bool Pass { get; private set; }
        public int MoveIndex { get; private set; }

        public static Move PassingMove()
        {
            return new Move()
            {
                Pass = true,
                MoveIndex = -1
            };
        }

        public static Move NormalMove(int index)
        {
            return new Move()
            {
                Pass = false,
                MoveIndex = index
            };
        }
    }

    public class Game
    {
        public int GameSize { get; } = 6;
        public int GameInit { get; } = 4;
        public Player CurrentPlayer { get; private set; } = Player.A;
        public Action OnGameOver = () => { };

        public List<int> playerAHoles { get; private set; }
        public int playerAWell { get; private set; }

        public List<int> playerBHoles { get; private set; }
        public int playerBWell { get; private set; }

        public bool PassFlag { get; private set; }= false;

        public Game()
        {
            Reset();
        }

        public List<Move> GetAvailableMoves()
        {
            List<Move> result = new List<Move>();
            if (PassFlag)
            {
                result.Add(Mancala.Move.PassingMove());
                return result;
            }
            else
            {
                List<int> holes = CurrentPlayer == Player.A ? playerAHoles : playerBHoles;
                foreach (var hole in holes)
                {
                    if (hole > 0)
                    {
                        result.Add(Mancala.Move.NormalMove(hole));
                    }
                }

                return result;
            }
        }

        public bool IsValidMove(int index)
        {
            List<int> holes = CurrentPlayer == Player.A ? playerAHoles : playerBHoles;
            return holes[index] > 0;
        }

        public void Reset()
        {
            playerAHoles = new List<int>();
            playerBHoles = new List<int>();

            for (int i = 0; i < GameSize; i++)
            {
                playerAHoles.Add(GameInit);
                playerBHoles.Add(GameInit);
            }

            playerAWell = 0;
            playerBWell = 0;

            CurrentPlayer = Player.A;
        }

        public bool Move(Player p, Move move)
        {
            if (p != CurrentPlayer)
            {
                return false;
            }

            if (move.Pass)
            {
                PassFlag = false;
                CurrentPlayer = PlayerUtils.Other(CurrentPlayer);
                return true;
            }

            Game nextState = TryMove(p, move.MoveIndex, out bool error);
            if (!error)
            {
                LoadState(nextState);
            }
            else
            {
                throw new InvalidOperationException("Error occurred while moving");
            }

            if (IsGameOver())
            {
                OnGameOver?.Invoke();
            }

            return true;
        }

        public Game TryMove(Player p, int index, out bool error)
        {
            Game result = new Game();
            result.LoadState(this);

            error = false;
            List<List<int>> holes = new List<List<int>>();
            holes.Add(result.playerAHoles);
            if (p == Player.A)
            {
                holes.Add(result.playerBHoles);
            }
            else
            {
                holes.Insert(0, result.playerBHoles);
            }

            if (holes[0][index] == 0 || p != CurrentPlayer)
            {
                error = true;
                return null;
            }

            int stones = holes[0][index];
            holes[0][index] = 0;
            Player nextWellOwner = p;
            bool keepTurnFlag = true;

            while (stones > 0)
            {
                index++;
                keepTurnFlag = false;
                if (index == GameSize)
                {
                    holes.Add(holes[0]);
                    holes.RemoveAt(0);
                    index = 0;
                    if (nextWellOwner == p)
                    {
                        stones--;
                        if (p == Player.A)
                        {
                            result.playerAWell++;
                        }
                        else
                        {
                            result.playerBWell++;
                        }

                        keepTurnFlag = true;
                    }
                    nextWellOwner = PlayerUtils.Other(nextWellOwner);
                }

                if (stones > 0)
                {
                    holes[0][index]++;
                    stones--;

                    if (holes[0][index] == 1 && nextWellOwner == p && stones == 0)
                    {
                        // Strike
                        keepTurnFlag = false;
                        if (p == Player.A)
                        {
                            result.playerAWell += holes[1][GameSize - index - 1];
                            result.playerAWell += holes[0][index];
                        }
                        else
                        {
                            result.playerBWell += holes[1][GameSize - index - 1];
                            result.playerBWell += holes[0][index];
                        }

                        holes[1][GameSize - index - 1] = 0;
                        holes[0][index] = 0;
                    }
                }
            }

            result.CurrentPlayer = PlayerUtils.Other(result.CurrentPlayer);

            // Force pass from other player
            PassFlag = keepTurnFlag;

            return result;
        }

        public void LoadState(Game from)
        {
            CurrentPlayer = from.CurrentPlayer;
            playerAHoles = new List<int>(from.playerAHoles);
            playerAWell = from.playerAWell;
            playerBHoles = new List<int>(from.playerBHoles);
            playerBWell = from.playerBWell;
        }

        public bool IsGameOver()
        {
            if (playerAHoles.All(i => i == 0) || playerBHoles.All(i => i == 0))
            {
                return true;
            }

            return false;
        }

        public Player? GetWinningPlayer()
        {
            int playerASum = playerAWell + playerAHoles.Sum(h => h);
            int playerBSum = playerBWell + playerBHoles.Sum(h => h);

            if (playerASum == playerBSum)
            {
                return null;
            }

            if (playerASum > playerBSum)
            {
                return Player.A;
            }
            else
            {
                return Player.B;
            }
        }

        public override string ToString()
        {
            string res = "";

            res += "Player A well: " + playerAWell + "\n     ";
            for (int i = playerAHoles.Count - 1; i >= 0; i--)
            {
                res += playerAHoles[i] + " ";
            }
            res += "<- A\n";
            res += "B -> ";
            for (int i = 0; i < playerBHoles.Count; i++)
            {
                res += playerBHoles[i] + " ";
            }

            res += "\nPlayer B well: " + playerBWell + "\n";

            return res;
        }
    }
}
