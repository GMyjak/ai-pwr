using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown playerADropdown;

    [SerializeField]
    private TMP_Dropdown playerBDropdown;

    [SerializeField]
    private Button resetButton;

    [SerializeField]
    private GameManager gm;

    void Awake()
    {
        resetButton.onClick.AddListener(OnResetButtonClicked);
    }

    private void OnResetButtonClicked()
    {
        gm.Reset(playerADropdown.value, playerBDropdown.value);
    }
}
