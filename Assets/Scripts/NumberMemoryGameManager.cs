using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberMemoryGameManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform gridParent;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI livesText;
    public GameObject gameCompletedPanel;

    public int currentLevel = 1;
    public int maxLevel = 10;
    public int checkpointLevel = 1;
    public int maxLives = 3;
    private int currentLives;

    public bool allowClick = false;
    public int nextExpectedNumber = 1;
    private int gridSize = 3;

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
            Destroy(child.gameObject);

        gridSize = GetGridSizeForLevel(currentLevel);
        nextExpectedNumber = 1;
        currentLives = maxLives;
        UpdateLivesUI();

        AdaptiveGrid adaptiveGrid = gridParent.GetComponent<AdaptiveGrid>();
        if (adaptiveGrid != null)
            adaptiveGrid.UpdateGridSize(gridSize);

        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize;

        int totalTiles = gridSize * gridSize;

        // STEP 1: Shuffle numbers and assign one to each unique row
        List<int> rowIndices = new List<int>();
        for (int i = 0; i < gridSize; i++) rowIndices.Add(i);
        Shuffle(rowIndices); // randomize which row gets which number

        // Map from row index to (column, number) assignment
        Dictionary<int, (int col, int number)> numberedTiles = new Dictionary<int, (int, int)>();
        for (int number = 1; number <= gridSize; number++)
        {
            int row = rowIndices[number - 1];
            int col = Random.Range(0, gridSize);
            numberedTiles[row] = (col, number);
        }

        // STEP 2: Create grid and assign tile values
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                GameObject tile = Instantiate(tilePrefab, gridParent);
                NumberMemoryTile tileScript = tile.GetComponent<NumberMemoryTile>();
                tileScript.gameManager = this;

                // Assign number if this is the chosen tile
                if (numberedTiles.ContainsKey(row) && numberedTiles[row].col == col)
                {
                    tileScript.numberValue = numberedTiles[row].number;
                    tileScript.isCorrect = true;
                }
                else
                {
                    tileScript.numberValue = 0;
                    tileScript.isCorrect = false;
                }

                tileScript.HideNumber();
                tile.GetComponent<Button>().onClick.AddListener(tileScript.OnClick);
            }
        }

        if (levelText != null)
            levelText.text = "Level: " + currentLevel;


        StartCoroutine(FlashCorrectTiles());
    }




    IEnumerator FlashCorrectTiles()
    {
        allowClick = false;

        foreach (Transform tile in gridParent)
        {
            var script = tile.GetComponent<NumberMemoryTile>();
            if (script.numberValue > 0)
                script.ShowNumber();
        }

        yield return new WaitForSeconds(1.5f);

        foreach (Transform tile in gridParent)
            tile.GetComponent<NumberMemoryTile>().HideNumber();

        allowClick = true;
    }

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }



    int GetGridSizeForLevel(int level)
    {
        if (level <= 1) return 3;
        if (level <= 3) return 4;
        if (level <= 5) return 5;
        if (level <= 7) return 6;
        if (level <= 9) return 7;
        return 8;
    }

    public void OnCorrectTileClicked()
    {
        nextExpectedNumber++;

        if (nextExpectedNumber > gridSize)
        {
            AudioManager.Instance.PlayLevelCompleteSound();
            Invoke(nameof(NextLevel), 1f);
        }
    }

    public void ReduceLife()
    {
        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            Debug.Log("Out of Lives — Restart from checkpoint or show GameOver.");
            // TODO: Show game over panel if needed
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;
    }



    void NextLevel()
    {
        currentLevel++;
        if (currentLevel == 6 || currentLevel == 10)
            checkpointLevel = currentLevel;

        if (currentLevel > maxLevel)
        {
            gameCompletedPanel.SetActive(true);
            AudioManager.Instance.PlayGameCompletedSound();
        }
        else
        {
            GenerateGrid();
        }
    }
}
