using System;
using System.Collections.Generic;
using Mancala;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField]
    public List<BoardEntity> playerAHoles;
    [SerializeField]
    public List<BoardEntity> playerBHoles;
    [SerializeField]
    public BoardEntity playerAWell;
    [SerializeField]
    public BoardEntity playerBWell;

    private List<Button> holeAButtons;
    private List<Button> holeBButtons;

    public Action<int> OnClick = (_) => { };

    void Awake()
    {
        holeAButtons = new List<Button>();
        holeBButtons = new List<Button>();

        for (var i = 0; i < playerAHoles.Count; i++)
        {
            var hole = playerAHoles[i];
            var button = hole.gameObject.GetComponent<Button>();
            AddListenerToButton(button, i);
            holeAButtons.Add(button);
        }

        for (var i = 0; i < playerBHoles.Count; i++)
        {
            var hole = playerBHoles[i];
            var button = hole.gameObject.GetComponent<Button>();
            AddListenerToButton(button, i);
            holeBButtons.Add(button);
        }
    }

    private void AddListenerToButton(Button b, int index)
    {
        b.onClick.AddListener(() =>
        {
            OnClick?.Invoke(index);
        });
    }

    public void Reset()
    {
        playerAHoles.ForEach(h => h.Reset());
        playerBHoles.ForEach(h => h.Reset());
        playerAWell.Reset();
        playerBWell.Reset();
    }

    public void DisableActions(Player player)
    {
        if (player == Player.A)
        {
            holeAButtons.ForEach(b => b.interactable = false);
        }
        else
        {
            holeBButtons.ForEach(b => b.interactable = false);
        }
    }

    public void EnableActions(Player player)
    {
        if (player == Player.A)
        {
            for (var i = 0; i < holeAButtons.Count; i++)
            {
                var b = holeAButtons[i];
                b.interactable = PlayerAHoles[i].Number > 0;
            }
        }
        else
        {
            for (var i = 0; i < holeBButtons.Count; i++)
            {
                var b = holeBButtons[i];
                b.interactable = PlayerBHoles[i].Number > 0;
            }
        }
    }

    #region Wrappers

    public List<BoardEntity> PlayerAHoles
    {
        get => playerAHoles;
        set => playerAHoles = value;
    }

    public List<BoardEntity> PlayerBHoles
    {
        get => playerBHoles;
        set => playerBHoles = value;
    }

    public BoardEntity PlayerAWell
    {
        get => playerAWell;
        set => playerAWell = value;
    }

    public BoardEntity PlayerBWell
    {
        get => playerBWell;
        set => playerBWell = value;
    }

    #endregion
}
