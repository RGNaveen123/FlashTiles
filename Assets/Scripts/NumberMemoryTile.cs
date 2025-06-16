using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NumberMemoryTile : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public int numberValue;
    public bool isClicked = false;
    public NumberMemoryGameManager gameManager;

    public void Show()
    {
        numberText.text = numberValue.ToString();
    }

    public void Hide()
    {
        numberText.text = "";
        isClicked = false;
    }

    public void OnClick()
    {
        if (isClicked || !gameManager.allowClick) return;

        isClicked = true;

        if (gameManager.IsCorrectNumber(numberValue))
        {
            Show();
            gameManager.TileClickedCorrect(numberValue);
            AudioManager.Instance.PlayCorrectSound();
        }
        else
        {
            Show(); // Optional: briefly show incorrect number
            gameManager.ReduceLife();
            AudioManager.Instance.PlayWrongSound();
        }
    }
}
