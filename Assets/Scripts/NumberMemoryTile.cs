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
        if (!gameManager.allowClick) return;

        if (numberValue == gameManager.nextExpectedNumber)
        {
            isClicked = true;  // Only lock tile after correct selection
            ShowNumber();
            AudioManager.Instance.PlayCorrectSound();
            gameManager.OnCorrectTileClicked();
        }
        else if (numberValue == 0) // Blank tile
        {
            isClicked = true;  // Lock wrong tiles so they can't be spammed
            ShowX();
            AudioManager.Instance.PlayWrongSound();
            gameManager.ReduceLife();
        }
        else // Number tile, but wrong order
        {
            // Don't lock it yet — allow retry
            AudioManager.Instance.PlayWrongSound();
            gameManager.ReduceLife();
            Debug.Log("Wrong order — try again later.");
        }
    }
}
