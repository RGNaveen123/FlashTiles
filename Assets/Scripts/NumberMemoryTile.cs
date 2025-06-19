using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NumberMemoryTile : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public Image xMarkImage;

    public int numberValue; // 0 if empty
    public bool isCorrect = false;

    public bool isClicked = false;      // Was clicked once
    public bool isRevealed = false;     // Correct tile shown
    public bool isMarkedWrong = false;  // X marked

    public NumberMemoryGameManager gameManager;

    public void ShowNumber()
    {
        numberText.text = numberValue > 0 ? numberValue.ToString() : "";
        xMarkImage.gameObject.SetActive(false);
        isRevealed = true;
    }

    public void HideNumber()
    {
        numberText.text = "";
        xMarkImage.gameObject.SetActive(false);
        isClicked = false;
        isRevealed = false;
        isMarkedWrong = false;
    }

    public void ShowX()
    {
        xMarkImage.gameObject.SetActive(true);
        isMarkedWrong = true;
    }

    public void OnClick()
    {
        if (!gameManager.allowClick)
            return;

        //  CASE 1: Tile already revealed as correct (do nothing)
        if (isRevealed)
            return;

        //  CASE 2: Tile already marked with X (do nothing)
        if (isMarkedWrong)
            return;

        //  CASE 3: Correct number in correct sequence
        if (numberValue == gameManager.nextExpectedNumber)
        {
            isClicked = true;
            ShowNumber();
            AudioManager.Instance.PlayCorrectSound();
            gameManager.OnCorrectTileClicked();
        }
        //  CASE 4: Number tile, but out of order
        else if (numberValue > 0)
        {
            AudioManager.Instance.PlayWrongSound();
            gameManager.ReduceLife();
        }
        //  CASE 5: Empty tile
        else if (numberValue == 0)
        {
            isClicked = true;
            ShowX();
            AudioManager.Instance.PlayWrongSound();
            gameManager.ReduceLife();
        }
    }
}
