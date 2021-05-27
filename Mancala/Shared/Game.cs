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

    public struct StoneAddition
    {
        public Player PlayerFrom;
        public int PlayerFromIndex;
        public Player PlayerTo;
        public int PlayerToIndex;
    }

    public class Game
    {
        public int GameSize { get; } = 6;
        public int GameInit { get; } = 4;
        public Player CurrentPlayer { get; private set; } = Player.A;

        public List<int> PlayerAHoles { get; private set; }
        public int PlayerAWell { get; private set; }

        public List<int> PlayerBHoles { get; private set; }
        public int PlayerBWell { get; private set; }

        public bool PassFlag { get; private set; } = false;

        public Action OnGameOver = () => { };
        public Action<List<StoneAddition>> OnMove = (_) => { };

        public Game()
        {
            Reset();
        }

        // Movement

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
                List<int> holes = CurrentPlayer == Player.A ? PlayerAHoles : PlayerBHoles;
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
            List<int> holes = CurrentPlayer == Player.A ? PlayerAHoles : PlayerBHoles;
            return holes[index] > 0;
        }

        public void MakeRandomMove()
        {
            var moves = GetAvailableMoves();
            Random rng = new Random();
            moves = moves.OrderBy(m => rng.Next()).ToList();
            Move(CurrentPlayer, moves[0]);
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

            Game nextState = TryMove(p, move.MoveIndex, out bool error, out var moves);
            if (!error)
            {
                LoadState(nextState);
                OnMove?.Invoke(moves);
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

        public Game TryMove(Player p, int index, out bool error, out List<StoneAddition> moves)
        {
            moves = null;
            Game result = new Game();
            result.LoadState(this);

            error = false;
            List<List<int>> holes = new List<List<int>>();
            holes.Add(result.PlayerAHoles);
            if (p == Player.A)
            {
                holes.Add(result.PlayerBHoles);
            }
            else
            {
                holes.Insert(0, result.PlayerBHoles);
            }

            if (holes[0][index] == 0 || p != CurrentPlayer)
            {
                error = true;
                return null;
            }

            int stones = holes[0][index];
            holes[0][index] = 0;
            int initialIndex = index;
            Player initialPlayer = CurrentPlayer;
            moves = new List<StoneAddition>();
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
                    if (nextWellOwner == p)
                    {
                        stones--;
                        moves.Add(new StoneAddition()
                        {
                            PlayerFrom = initialPlayer,
                            PlayerTo = nextWellOwner,
                            PlayerFromIndex = initialIndex,
                            PlayerToIndex = index,
                        });

                        if (p == Player.A)
                        {
                            result.PlayerAWell++;
                        }
                        else
                        {
                            result.PlayerBWell++;
                        }

                        index = 0;
                        keepTurnFlag = true;
                    }
                    nextWellOwner = PlayerUtils.Other(nextWellOwner);
                }

                if (stones > 0)
                {
                    holes[0][index]++;
                    stones--;
                    keepTurnFlag = false;

                    moves.Add(new StoneAddition()
                    {
                        PlayerFrom = initialPlayer,
                        PlayerTo = nextWellOwner,
                        PlayerFromIndex = initialIndex,
                        PlayerToIndex = index,
                    });

                    if (holes[0][index] == 1 && nextWellOwner == p && stones == 0)
                    {
                        // Strike
                        if (p == Player.A)
                        {
                            result.PlayerAWell += holes[1][GameSize - index - 1];
                            result.PlayerAWell += holes[0][index];
                        }
                        else
                        {
                            result.PlayerBWell += holes[1][GameSize - index - 1];
                            result.PlayerBWell += holes[0][index];
                        }

                        holes[1][GameSize - index - 1] = 0;
                        holes[0][index] = 0;
                    }
                }
            }

            result.CurrentPlayer = PlayerUtils.Other(result.CurrentPlayer);

            // Force pass from other player
            PassFlag = keepTurnFlag;

            //OnMove?.Invoke(moves);

            return result;
        }

        #region StateManagement
        // State management

        public bool IsGameOver()
        {
            if (PlayerAHoles.All(i => i == 0) || PlayerBHoles.All(i => i == 0))
            {
                return true;
            }

            return false;
        }

        public Player? GetWinningPlayer()
        {
            int playerASum = PlayerAWell + PlayerAHoles.Sum(h => h);
            int playerBSum = PlayerBWell + PlayerBHoles.Sum(h => h);

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

        #endregion

        #region Utils

        // Utils

        public void Reset()
        {
            PlayerAHoles = new List<int>();
            PlayerBHoles = new List<int>();

            for (int i = 0; i < GameSize; i++)
            {
                PlayerAHoles.Add(GameInit);
                PlayerBHoles.Add(GameInit);
            }

            PlayerAWell = 0;
            PlayerBWell = 0;

            CurrentPlayer = Player.A;
        }
        
        public void LoadState(Game from)
        {
            CurrentPlayer = from.CurrentPlayer;
            PlayerAHoles = new List<int>(from.PlayerAHoles);
            PlayerAWell = from.PlayerAWell;
            PlayerBHoles = new List<int>(from.PlayerBHoles);
            PlayerBWell = from.PlayerBWell;
        }

        public override string ToString()
        {
            string res = "";

            res += "Player A well: " + PlayerAWell + "\n     ";
            for (int i = PlayerAHoles.Count - 1; i >= 0; i--)
            {
                res += PlayerAHoles[i] + " ";
            }
            res += "<- A\n";
            res += "B -> ";
            for (int i = 0; i < PlayerBHoles.Count; i++)
            {
                res += PlayerBHoles[i] + " ";
            }

            res += "\nPlayer B well: " + PlayerBWell + "\n";

            return res;
        }

        #endregion
    }
}
