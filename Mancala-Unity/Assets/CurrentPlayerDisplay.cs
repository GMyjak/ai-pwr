using Mancala;
using TMPro;
using UnityEngine;

public class CurrentPlayerDisplay : MonoBehaviour
{
    private TMP_Text displayText;

    [SerializeField]
    private Color playerAColor;
    [SerializeField]
    private Color playerBColor;

    void Awake()
    {
        displayText = GetComponent<TMP_Text>();
    }

    public void Display(Player player)
    {
        if (player == Player.A)
        {
            displayText.text = "A";
            displayText.color = playerAColor;
        }
        else
        {
            displayText.text = "B";
            displayText.color = playerBColor;
        }
    }
}
