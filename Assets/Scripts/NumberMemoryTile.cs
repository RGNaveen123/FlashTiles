using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NumberMemoryTile : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public Image xMarkImage;

    public int numberValue;
    public bool isCorrect = false;
    public bool isClicked = false;

    public NumberMemoryGameManager gameManager;

    public void ShowNumber()
    {
        numberText.text = numberValue > 0 ? numberValue.ToString() : "";
        xMarkImage.gameObject.SetActive(false);
    }

    public void HideNumber()
    {
        numberText.text = "";
        xMarkImage.gameObject.SetActive(false);
        isClicked = false;
    }

    public void ShowX()
    {
        xMarkImage.gameObject.SetActive(true);
    }

    public void OnClick()
    {
        if (isClicked || !gameManager.allowClick) return;

        isClicked = true;

        if (numberValue == gameManager.nextExpectedNumber)
        {
            ShowNumber();
            AudioManager.Instance.PlayCorrectSound();
            gameManager.OnCorrectTileClicked();
        }
        else
        {
            ShowX();
            AudioManager.Instance.PlayWrongSound();
            gameManager.ReduceLife();
        }
    }
}
