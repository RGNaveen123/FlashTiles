using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NumberMemoryGameManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform gridParent;
    public int currentLevel = 1;
    public int maxLevel = 10;
    public int checkpointLevel = 1;

    public int maxLives = 3;
    private int currentLives;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI livesText;
    public GameObject gameCompletedPanel;

    public bool allowClick = false;
    private int correctTilesClicked = 0;
    private int totalCorrectTiles = 0;
    private int nextExpectedNumber = 1;

    public int gridSize = 3;

    void Start()
    {
        AudioManager.Instance.PlayBGM();
        StartCoroutine(DelayedGridGeneration());
    }

    IEnumerator DelayedGridGeneration()
    {
        yield return null;
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        gridSize = GetGridSizeForLevel(currentLevel);
        totalCorrectTiles = gridSize * gridSize;
        correctTilesClicked = 0;
        nextExpectedNumber = 1;
        currentLives = maxLives;
        UpdateLivesUI();
        allowClick = false;

        AdaptiveGrid adaptiveGrid = gridParent.GetComponent<AdaptiveGrid>();
        if (adaptiveGrid != null)
            adaptiveGrid.UpdateGridSize(gridSize);

        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize;

        List<int> numbers = new List<int>();

        if (currentLevel < 6)
        {
            for (int i = 0; i < totalCorrectTiles; i++)
                numbers.Add(Random.Range(1, 4)); // Random 1–3
        }
        else
        {
            for (int i = 1; i <= totalCorrectTiles; i++)
                numbers.Add(i);
            Shuffle(numbers);
        }

        for (int i = 0; i < totalCorrectTiles; i++)
        {
            GameObject tile = Instantiate(tilePrefab, gridParent);
            NumberMemoryTile tileScript = tile.GetComponent<NumberMemoryTile>();
            tileScript.numberValue = numbers[i];
            tileScript.numberText = tile.GetComponentInChildren<TextMeshProUGUI>();
            tileScript.gameManager = this;
            tileScript.Hide();

            tile.GetComponent<Button>().onClick.AddListener(tileScript.OnClick);
        }

        StartCoroutine(FlashAllTiles());
        if (levelText != null)
            levelText.text = "Level: " + currentLevel;
    }

    IEnumerator FlashAllTiles()
    {
        allowClick = false;

        foreach (Transform t in gridParent)
        {
            t.GetComponent<NumberMemoryTile>().Show();
        }

        yield return new WaitForSeconds(1.5f);

        foreach (Transform t in gridParent)
        {
            t.GetComponent<NumberMemoryTile>().Hide();
        }

        allowClick = true;
    }

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    int GetGridSizeForLevel(int level)
    {
        if (level == 1) return 3;
        else if (level == 2 || level == 3) return 4;
        else if (level == 4 || level == 5) return 5;
        else if (level == 6 || level == 7) return 6;
        else if (level == 8 || level == 9) return 7;
        else return 8;
    }

    public bool IsCorrectNumber(int num)
    {
        return num == nextExpectedNumber;
    }

    public void TileClickedCorrect(int number)
    {
        correctTilesClicked++;
        nextExpectedNumber++;

        if (correctTilesClicked >= totalCorrectTiles)
        {
            Debug.Log("Level Completed!");
            AudioManager.Instance.PlayLevelCompleteSound();
            Invoke(nameof(GoToNextLevel), 1f);
        }
    }

    public void ReduceLife()
    {
        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            Debug.Log("Game Over");
            // You can add a game over panel
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;
    }

    void GoToNextLevel()
    {
        currentLevel++;

        if (currentLevel == 6 || currentLevel == 10)
        {
            checkpointLevel = currentLevel;
        }

        if (currentLevel > maxLevel)
        {
            Debug.Log("Game Completed");
            AudioManager.Instance.PlayGameCompletedSound();
            if (gameCompletedPanel != null)
                gameCompletedPanel.SetActive(true);
        }
        else
        {
            GenerateGrid();
        }
    }
}
