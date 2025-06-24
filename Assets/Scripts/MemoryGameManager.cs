using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MemoryGameManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform gridParent;
    public Sprite hiddenSprite;
    public List<Sprite> shownSprites;  // pool of images
    public int currentLevel = 1;
    public int checkpointLevel = 1;
    public int maxLevel = 10;
    private int totalCorrectTiles = 0;
    private int correctTilesClicked = 0;
    public TextMeshProUGUI levelText;  // For TMP
    public TextMeshProUGUI livesText;  // UI to display lives
    public int maxLives = 3; // Total Number of lives
    private int currentLives;
    [HideInInspector] public bool allowClick = false; //  Prevent clicks until flash ends
    private bool gameIsPaused = false; // Pausing the game
    public Sprite wrongSprite;

    public float timePerLevel = 10f; // You can adjust this per level later
    private float timeLeft;
    private bool isTimerRunning = false;
    public TextMeshProUGUI timerText;  // Drag the TimerText UI here
    public GameObject timerUI;

    // UI
    public GameObject timesUpPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject gameCompletedPanel;
    public GameObject pauseButton;
    public Animator checkpointToastAnimator;  // Drag your toast panel here

    //Time 
    [Header("Speed Run Timer")]
    private float totalRunTime = 0f;
    private bool isRunTimerRunning = false;
    private bool isRunTimerPaused = false;

    public TextMeshProUGUI currentRunTimeText;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI bestRunTimeText;
    public TextMeshProUGUI motivationalText;

    // Modekey 
    public string modeKey = "Memory";

    // timer reset point
    private float checkpointRunTime = 0f;



    public int gridSize = 3;  // starting with 3x3


    void Start()
    {
        // Load custom settings
        bool isSpeedRun = PlayerPrefs.GetInt("EnableSpeedRun", 1) == 1;
        bool isTimerOn = PlayerPrefs.GetInt("EnableTimer", 1) == 1;
        float savedTime = PlayerPrefs.GetFloat("TimerDuration", 15f);
        int savedLives = PlayerPrefs.GetInt("NumLives", 3);

        // Apply them
        if (!isSpeedRun) isRunTimerRunning = false;   // Disables speed run timer
        if (!isTimerOn) timePerLevel = 99999f;        // Set timer really high (or skip StartTimer)
        maxLives = savedLives;
        timePerLevel = savedTime;

        totalRunTime = 0f;
        isRunTimerRunning = PlayerPrefs.GetInt("EnableSpeedRun", 1) == 1;
        StartCoroutine(DelayedGridGeneration());

    }

    IEnumerator DelayedGridGeneration()
    {
        yield return null;
        GenerateGrid();
    }

    IEnumerator FlashCorrectTilesThenEnableClicks(List<MemoryTile> correctTiles)
    {
        allowClick = false;
        isRunTimerPaused = true;

        // Short delay to let player get ready
        yield return new WaitForSeconds(1f);

        // Show correct tiles
        foreach (var tile in correctTiles)
        {
            tile.Show();
        }

        yield return new WaitForSeconds(1f);

        foreach (var tile in correctTiles)
        {
            tile.Hide();
        }

        allowClick = true;
        isRunTimerPaused = false;

        StartTimer(); // Timer for level countdown starts now
    }




    public void GenerateGrid()
    {
        // Clear previous tiles
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // Update grid size based on level
        gridSize = GetGridSizeForLevel(currentLevel);

        // Adjust tile size dynamically
        AdaptiveGrid adaptiveGrid = gridParent.GetComponent<AdaptiveGrid>();
        if (adaptiveGrid != null)   
        {
            adaptiveGrid.UpdateGridSize(gridSize);
        }
        else
        {
            Debug.LogError("AdaptiveGrid component not found on gridParent.");
        }


        // Update Grid Layout constraint
        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize;

        int totalTiles = gridSize * gridSize;
        List<int> correctIndices = new List<int>();
        correctTilesClicked = 0;
        allowClick = true;  // Re-enable clicks for new level
        // Handling life as per clicks
        currentLives = maxLives;
        UpdateLivesUI();
        timerUI.SetActive(false); // Hide timer while flashing




        // Pick one correct tile per row
        for (int row = 0; row < gridSize; row++)
        {
            int randomCol = Random.Range(0, gridSize);
            int index = row * gridSize + randomCol;
            correctIndices.Add(index);
        }

        totalCorrectTiles = correctIndices.Count;


        List<MemoryTile> correctTiles = new List<MemoryTile>();


        // Create grid tiles
        for (int i = 0; i < totalTiles; i++)
        {
            GameObject tile = Instantiate(tilePrefab, gridParent);
            MemoryTile memoryTile = tile.GetComponent<MemoryTile>();
            memoryTile.hiddenSprite = hiddenSprite;
            memoryTile.shownSprite = shownSprites[0];
            memoryTile.wrongSprite = wrongSprite;  // Optional if showing X
            memoryTile.Hide();
            memoryTile.gameManager = this;

            if (correctIndices.Contains(i))
            {
                memoryTile.isCorrect = true;
                correctTiles.Add(memoryTile);
            }

            tile.GetComponent<Button>().onClick.AddListener(memoryTile.OnClick);
        }

        // Start flashing after tile creation
        StartCoroutine(FlashCorrectTilesThenEnableClicks(correctTiles));

        if (levelText != null)
            levelText.text = "Level: " + currentLevel;

    }

    IEnumerator FlashTile(MemoryTile tile)
    {
        yield return new WaitForSeconds(1f);  // delay before flash
        tile.Show();
        yield return new WaitForSeconds(1f);  // show duration
        tile.Hide();
    }

    int GetGridSizeForLevel(int level)
    {
        if (level == 1) return 3;
        else if (level == 2 || level == 3) return 4;
        else if (level == 4 || level == 5) return 5;
        else if (level == 6 || level == 7) return 6;
        else if (level == 8 || level == 9) return 7;
        else return 8; // Level 10
    }

    public void GoToNextLevel()
    {
        currentLevel++;
        isTimerRunning = false;
        timerUI.SetActive(false);
        // Checkpoint logic
        if (currentLevel == 6)
        {
            checkpointLevel = currentLevel;
            checkpointRunTime = totalRunTime;
            Debug.Log("Checkpoint saved at Level " + checkpointLevel);

            // Show checkpoint UI
            ShowCheckpointToast();
        }

        // End game
        if (currentLevel > maxLevel)
        {
            // Optionally play a victory sound
            AudioManager.Instance.PlayGameCompletedSound();
            currentRunTimeText.gameObject.SetActive(false);
            isRunTimerRunning = false;
            allowClick = false;

            // Show Game Completed Panel
            if (gameCompletedPanel != null)
                gameCompletedPanel.SetActive(true);

            // Round and show current time
            if (currentTimeText != null)
                currentTimeText.text = totalRunTime.ToString("F1") + "s";

            // Get saved best time For memory mode
            string bestTimeKey = "BestTime_" + modeKey; // e.g., BestTime_Memory
            float savedBest = PlayerPrefs.GetFloat(bestTimeKey, float.MaxValue);
            bool isNewBest = false;

            if (totalRunTime < savedBest)
            {
                PlayerPrefs.SetFloat(bestTimeKey, totalRunTime);
                PlayerPrefs.Save();
                isNewBest = true;
            }

            // Show best time
            if (bestRunTimeText != null)
            {
                float bestTime = PlayerPrefs.GetFloat("BestTime_" + modeKey, float.MaxValue);
                if (bestTime == float.MaxValue)
                    bestRunTimeText.text = "—";
                else
                    bestRunTimeText.text = bestTime.ToString("F1") + "s";
            }


            // Show motivational message
            if (motivationalText != null)
            {
                if (isNewBest)
                    motivationalText.text = " New Personal Best!";

                else
                    motivationalText.text = " Try again to beat your best!";
            }
        }

        GenerateGrid();
    }

    public void RetryFromCheckpoint()
    {
        // Resume time (in case game was paused)
        Time.timeScale = 1f;

        AudioManager.Instance.PlayButtonSound();

        // Hide any open menus
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (timesUpPanel != null) timesUpPanel.SetActive(false);

        // Reset interaction state
        pauseButton.SetActive(true);
        allowClick = true;
        gameIsPaused = false;

        // Reset level and regenerate grid
        currentLevel = checkpointLevel;
        GenerateGrid();
    }

    public void TileClickedCorrect()
    {
        correctTilesClicked++;

        if (correctTilesClicked >= totalCorrectTiles)
        {
            allowClick = false; // Disable the tiles after the correct tiles are pressed succuessfully
            Debug.Log(" Level Completed!");
            AudioManager.Instance.PlayLevelCompleteSound();
            Invoke("GoToNextLevel", 1f); // Delay before next level
        }
    }

    // Updating live lifes
    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;
    }

    public void ReduceLife()
    {
        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            Debug.Log("Out of Lives! Restarting from checkpoint...");
            GameOver();
            //RetryFromCheckpoint();  // Or call GenerateGrid() if you want to retry same level
        }

        if (currentLevel < checkpointLevel)
        {
            // Player hadn't reached checkpoint yet
            totalRunTime = 0f;
        }
        else
        {
            // Player reached checkpoint earlier, restore time
            totalRunTime = checkpointRunTime;
        }
    }

    void StartTimer()
    {
        if (PlayerPrefs.GetInt("EnableTimer", 1) == 0)
            return; // Timer is OFF

        timerUI.SetActive(true); // Show timer when play starts
        timeLeft = timePerLevel;
        isTimerRunning = true;

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();

    }

    void Update()
    {
        if (isTimerRunning && !gameIsPaused)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = "Timer: " + Mathf.CeilToInt(timeLeft);

            float t = Mathf.InverseLerp(timePerLevel, 0f, timeLeft);
            timerText.color = Color.Lerp(Color.white, Color.red, t);

            if (timeLeft <= 0)
            {
                TimeOut();
            }
        }

        if (isRunTimerRunning && !gameIsPaused && !isRunTimerPaused)
        {
            totalRunTime += Time.deltaTime;

            if (currentRunTimeText != null)
                currentRunTimeText.text = "Time: " + totalRunTime.ToString("F1") + "s";
        }


    }

    void TimeOut()
    {
        isTimerRunning = false;
        allowClick = false;
        gameIsPaused = true;
        

        if (currentLevel < checkpointLevel)
        {
            // Player hadn't reached checkpoint yet
            totalRunTime = 0f;
        }
        else
        {
            // Player reached checkpoint earlier, restore time
            totalRunTime = checkpointRunTime;
        }

        AudioManager.Instance.PlayLevelFailSound();

        timesUpPanel.SetActive(true); // Show the panel
    }



    void ShowTimesUpPanel()
    {
        timesUpPanel.SetActive(true);


        if (timeLeft <= 0)
        {
            isTimerRunning = false;
            AudioManager.Instance.PlayLevelFailSound();
            ShowTimesUpPanel();
        }

    }

    void HideTimesUpPanel()
    {
        timesUpPanel.SetActive(false);
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlayButtonSound();
        Time.timeScale = 2f;
        Debug.Log("Exit Pressed");
        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    public void PauseGame()
    {
        AudioManager.Instance.PlayButtonSound();
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlayButtonSound();
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        AudioManager.Instance.PlayLevelFailSound();
    }

    private bool checkpointToastShowing = false;

    public void ShowCheckpointToast()
    {
        if (checkpointToastAnimator != null && !checkpointToastShowing)
        {
            checkpointToastShowing = true;

            // Activate and play SlideIn
            checkpointToastAnimator.gameObject.SetActive(true);
            checkpointToastAnimator.Play("SlideIn", -1, 0f);

            // Schedule slide out after delay
            Invoke(nameof(TriggerSlideOut), 2f);
        }
    }

    private void TriggerSlideOut()
    {
        if (checkpointToastAnimator != null)
        {
            checkpointToastAnimator.Play("SlideOutRight");
            Invoke(nameof(HideCheckpointToast), 1f); // Hide toast panel after SlideOut finishes
        }
    }

    private void HideCheckpointToast()
    {
        if (checkpointToastAnimator != null)
        {
            checkpointToastAnimator.gameObject.SetActive(false);
            checkpointToastShowing = false;
        }
    }

    public void GoToHome()
    {
        AudioManager.Instance.PlayButtonSound();
        HideAllGamePanels();
        isTimerRunning = false;
        gameIsPaused = false;
        allowClick = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGameFromBeginning()
    {
        AudioManager.Instance.PlayButtonSound();

        // Reset variables
        currentLevel = 1;
        checkpointLevel = 1;
        totalRunTime = 0f;

        isTimerRunning = false;
        isRunTimerRunning = false;

        Time.timeScale = 1f; // Just in case

        SceneManager.LoadScene("MemoryGameScene"); // Your scene name here
    }

    private void HideAllGamePanels()
    {
        pauseMenuPanel.SetActive(false);
        timesUpPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameCompletedPanel.SetActive(false);
        checkpointToastAnimator.gameObject.SetActive(false);
        pauseButton.SetActive(false);
        timerUI.SetActive(false);
    }

    public void LoadGameModeMenu()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("ModeSelector"); // Send me back to the Game Mode Selector
    }


}
