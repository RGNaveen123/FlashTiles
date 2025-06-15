using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class MemoryTile : MonoBehaviour
{
    public Image tileImage;
    public Sprite hiddenSprite;
    public Sprite shownSprite;
    public bool isCorrect = false;

    private bool isClicked = false;
    //private MemoryGameManager gameManager;

    public Sprite wrongSprite;  //  New - assign this from Inspector

    public MemoryGameManager gameManager;  //  Reference to the manager


    private void Awake()
    {
        if (tileImage == null)
            tileImage = transform.Find("TileImage").GetComponent<Image>();

        gameManager = GetComponent<MemoryGameManager>();
    }

    public void Show()
    {
        tileImage.sprite = shownSprite;
    }

    public void Hide()
    {
        isClicked = false;
        tileImage.sprite = hiddenSprite;
    }

    public void OnClick()
    {
        if (!gameManager.allowClick || isClicked) return;

        isClicked = true;

        if (isCorrect)
        {
            Show();
            Debug.Log("Correct Tile!");
            gameManager.TileClickedCorrect();
            //gameManager.PlayCorrectSound();
            AudioManager.Instance.PlayCorrectSound();
        }
        else
        {
            tileImage.sprite = wrongSprite;
            Debug.Log("Wrong Tile!");
            gameManager.ReduceLife();
            //gameManager.PlayWrongSound();
            AudioManager.Instance.PlayWrongSound();

        }
    }

}
