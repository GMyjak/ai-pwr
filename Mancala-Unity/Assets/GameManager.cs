using System.Collections;
using System.Collections.Generic;
using Mancala;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private Board board;

    [SerializeField]
    private float blinkAnimationDelay = 0.15f;

    [SerializeField]
    private CurrentPlayerDisplay currentPlayerDisplay;

    public Game game { get; private set; }

    private bool moveLockFlag = false;

    private bool isPlayerAComputer = false;
    private bool isPlayerBComputer = false;

    void Awake()
    {
        game = new Game();

        game.OnGameOver += OnGameOver;
        game.OnMove += OnMove;

        board.OnClick = PerformMove;
    }

    void Start()
    {
        currentPlayerDisplay.Display(game.CurrentPlayer);
        board.DisableActions(game.CurrentPlayer);
        board.DisableActions(PlayerUtils.Other(game.CurrentPlayer));
        if (game.CurrentPlayer == Player.A && !isPlayerAComputer ||
            game.CurrentPlayer == Player.B && !isPlayerBComputer)
        {
            board.EnableActions(game.CurrentPlayer);
        }
        
    }

    void Update()
    {
        if (game.CurrentPlayer == Player.A && isPlayerAComputer && !moveLockFlag)
        {
            moveLockFlag = true;
            MoveAsAi();
        }
        else if (game.CurrentPlayer == Player.B && isPlayerBComputer && !moveLockFlag)
        {
            moveLockFlag = true;
            MoveAsAi();
        }
    }

    private void MoveAsAi()
    {
        MancalaAi mm = new MancalaAi(game, game.CurrentPlayer, Algo.MinMax);
        var move = mm.Move();
        PerformMove(move.MoveIndex);
        //game.Move(game.CurrentPlayer, move);
    }

    private void OnGameOver()
    {
        //TODO 
        Debug.Log($"Game over, winner: {game.GetWinningPlayer()}");
        board.DisableActions(Player.A);
        board.DisableActions(Player.B);
    }

    private void OnMove(List<StoneAddition> moves)
    {
        moveLockFlag = true;

        StartCoroutine(ProcessMoves(moves));
    }

    private IEnumerator ProcessMoves(List<StoneAddition> moves)
    {
        if (moves.Count > 0)
        {
            if (moves[0].PlayerFrom == Player.A)
            {
                board.playerAHoles[moves[0].PlayerFromIndex].Number = 0;
            }
            else
            {
                board.playerBHoles[moves[0].PlayerFromIndex].Number = 0;
            }

            yield return new WaitForSeconds(blinkAnimationDelay);
        }

        foreach (var move in moves)
        {
            if (move.PlayerTo == Player.A)
            {
                if (move.PlayerToIndex == game.GameSize)
                {
                    board.PlayerAWell.Number++;
                }
                else
                {
                    board.PlayerAHoles[move.PlayerToIndex].Number++;
                }
            }
            else
            {
                if (move.PlayerToIndex == game.GameSize)
                {
                    board.playerBWell.Number++;
                }
                else
                {
                    board.playerBHoles[move.PlayerToIndex].Number++;
                }
            }

            yield return new WaitForSeconds(blinkAnimationDelay);
        }

        if (board.playerAWell.Number != game.PlayerAWell)
        {
            board.playerAWell.Number = game.PlayerAWell;

            for (int i = 0; i < board.playerBHoles.Count; i++)
            {
                if (board.playerBHoles[i].Number != game.PlayerBHoles[i])
                {
                    board.playerBHoles[i].Number = game.PlayerBHoles[i];
                }
            }

            yield return new WaitForSeconds(blinkAnimationDelay);
        }
        else
        {
            for (int i = 0; i < board.playerBHoles.Count; i++)
            {
                if (board.playerBHoles[i].Number != game.PlayerBHoles[i])
                {
                    board.playerBHoles[i].Number = game.PlayerBHoles[i];
                }
            }

            yield return new WaitForSeconds(blinkAnimationDelay);
        }

        if (board.playerBWell.Number != game.PlayerBWell)
        {
            board.playerBWell.Number = game.PlayerBWell;

            for (int i = 0; i < board.playerAHoles.Count; i++)
            {
                if (board.playerAHoles[i].Number != game.PlayerAHoles[i])
                {
                    board.playerAHoles[i].Number = game.PlayerAHoles[i];
                }
            }

            yield return new WaitForSeconds(blinkAnimationDelay);
        }
        else
        {
            for (int i = 0; i < board.playerAHoles.Count; i++)
            {
                if (board.playerAHoles[i].Number != game.PlayerAHoles[i])
                {
                    board.playerAHoles[i].Number = game.PlayerAHoles[i];
                }
            }

            yield return new WaitForSeconds(blinkAnimationDelay);
        }

        currentPlayerDisplay.Display(game.CurrentPlayer);
        if (game.CurrentPlayer == Player.A && !isPlayerAComputer ||
            game.CurrentPlayer == Player.B && !isPlayerBComputer)
        {
            board.EnableActions(game.CurrentPlayer);
        }
        board.DisableActions(PlayerUtils.Other(game.CurrentPlayer));
        moveLockFlag = false;
    }

    private void PerformMove(int index)
    {
        if (!moveLockFlag)
        {
            game.Move(game.CurrentPlayer, Move.NormalMove(index));
            if (game.PassFlag)
            {
                game.Move(game.CurrentPlayer, Move.PassingMove());
            }

            board.DisableActions(game.CurrentPlayer);
            board.DisableActions(PlayerUtils.Other(game.CurrentPlayer));
        }
    }

    private void Reset()
    {
        board.Reset();
        Awake();
        Start();
    }

    public void Reset(int playerA, int playerB)
    {
        if (!moveLockFlag)
        {
            isPlayerAComputer = playerA == 1;
            isPlayerBComputer = playerB == 1;

            Reset();
        }
        else
        {
            Debug.Log("Cannot restart rn");
        }
    }
}
