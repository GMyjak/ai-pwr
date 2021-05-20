using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardEntity : MonoBehaviour
{
    [SerializeField]
    private TMP_Text displayText;
    [SerializeField]
    private Color stateChangedColor;
    [SerializeField, Range(0.2f, 2.0f)]
    private float blinkTime = 0.5f;
    [SerializeField]
    private int initialValue;

    private Color defaultColor;
    private Image entityImage;

    private int number;

    public int Number
    {
        get => number;
        set
        {
            number = value;
            displayText.text = number.ToString();
            StartCoroutine(BlinkCoroutine());
        }
    }

    private void Awake()
    {
        entityImage = GetComponent<Image>();
        defaultColor = entityImage.color;
        Reset();
    }

    private IEnumerator BlinkCoroutine()
    {
        float remainingTime = blinkTime;
        entityImage.color = stateChangedColor;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        entityImage.color = defaultColor;
    }

    public void Reset()
    {
        number = initialValue;
    }
}
