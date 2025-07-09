using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private bool gameIsPaused = false; // Pausing the game

    //Timer
    [Header("Timer")]
    public float timePerLevel = 15f; // Adjustable per level
    private float timeLeft;
    private bool isTimerRunning = false;
    public TextMeshProUGUI timerText;
    public GameObject timesUpPanel;

    // UI
    public GameObject pauseButton;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    //Time 
    [Header("Speed Run Timer")]
    private float totalRunTime = 0f;
    private bool isRunTimerRunning = false;
    private bool isRunTimerPaused = false;


    public TextMeshProUGUI currentRunTimeText;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI bestRunTimeText;
    public TextMeshProUGUI motivationalText;

    //Checkpoint
    public Animator checkpointToastAnimator; // Drag the Animator here

    //Modekey For finding which mode
    public string modeKey = "Number";

    // Timer reset point 
    private float checkpointRunTime = 0f;





    void Start()
    {

        // Load player settings
        bool speedRun = PlayerPrefs.GetInt("EnableSpeedRun", 1) == 1;
        bool timerOn = PlayerPrefs.GetInt("EnableTimer", 1) == 1;
        float dur = PlayerPrefs.GetFloat("TimerDuration", 15f);
        int lives = PlayerPrefs.GetInt("NumLives", 3);

        maxLives = lives;
        timePerLevel = timerOn ? dur : 99999f;
        isRunTimerRunning = speedRun;

        // Reset run-timer if starting from level 1
        if (currentLevel == 1)
        {
            PlayerPrefs.SetFloat("CheckpointTime", 0f);
            totalRunTime = 0f;
        }
        else if (isRunTimerRunning && currentLevel == checkpointLevel)
        {
            totalRunTime = PlayerPrefs.GetFloat("CheckpointTime", 0f);
        }
        else
        {
            totalRunTime = 0f;
        }

        // Hide speed-run timer if OFF
        if (!isRunTimerRunning && currentRunTimeText != null)
            currentRunTimeText.gameObject.SetActive(false);
        StartCoroutine(DelayedGridGeneration());
    }

    void Update()
    {
        if (isTimerRunning && !gameIsPaused)
        {
            timeLeft -= Time.deltaTime;

            if (timerText != null)
                timerText.text = "Timer: " + Mathf.CeilToInt(timeLeft).ToString();

            if (timeLeft <= 0)
            {
                TimeOut();
            }
        }

        if (isRunTimerRunning && !isRunTimerPaused)
        {
            totalRunTime += Time.deltaTime;

            if (currentRunTimeText != null)
                currentRunTimeText.text = "TIME: " + totalRunTime.ToString("F1") + "s";
        }

    }


    IEnumerator DelayedGridGeneration()
    {
        yield return null;
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        isTimerRunning = false;
        timerText.gameObject.SetActive(false); // hide until countdown starts

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
        timerText.gameObject.SetActive(false); //  hide the timer UI before flashing

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
        isRunTimerPaused = true;

        // OPTIONAL: short pause before flashing numbers
        yield return new WaitForSeconds(1f); // Freeze screen before flashing

        // Show all number tiles briefly
        foreach (Transform tile in gridParent)
        {
            var script = tile.GetComponent<NumberMemoryTile>();
            if (script.numberValue > 0)
                script.ShowNumber();
        }

        yield return new WaitForSeconds(1.5f); // Time while numbers are visible

        // Hide them again
        foreach (Transform tile in gridParent)
        {
            var script = tile.GetComponent<NumberMemoryTile>();
            script.HideNumber();
        }

        allowClick = true;
        isRunTimerPaused = false; //  Resume speed run
        StartTimer(); // starts the countdown timer for level
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
            GameOver();
        }


    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        timerText.gameObject.SetActive(false);   //  Number mode
        AudioManager.Instance.PlayLevelFailSound();
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;
    }



    void NextLevel()
    {
        currentLevel++;
        isTimerRunning = false;


        if (currentLevel == 6)
        {
            if (isRunTimerRunning)
                PlayerPrefs.SetFloat("CheckpointTime", totalRunTime);

            checkpointLevel = currentLevel;
            checkpointRunTime = totalRunTime;
            ShowCheckpointToast();
        }

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

            int finalSeconds = Mathf.RoundToInt(totalRunTime);
            LeaderboardManager.Instance.SubmitScore("number_speedrun", finalSeconds);


            // Round and show current time
            if (currentTimeText != null)
                currentTimeText.text = totalRunTime.ToString("F1") + "s";

            // Get saved best time
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

        else
        {
            GenerateGrid();
        }
    }

    public void RetryFromCheckpoint()
    {
        // Resume time (in case game was paused)
        Time.timeScale = 1f;

        currentRunTimeText.gameObject.SetActive(false);

        AudioManager.Instance.PlayButtonSound();
        if (isRunTimerRunning)
            totalRunTime = PlayerPrefs.GetFloat("CheckpointTime", 0f);

        // Hide any open menus
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (timesUpPanel != null) timesUpPanel.SetActive(false);

        // Reset interaction state
        pauseButton.SetActive(true);
        allowClick = true;
        gameIsPaused = false; //add it after i make the changes

        // Reset level and regenerate grid
        currentLevel = checkpointLevel;
        GenerateGrid();
    }

    public void RestartGameFromBeginning()
    {
        AudioManager.Instance.PlayButtonSound();
        currentRunTimeText.gameObject.SetActive(false);
        PlayerPrefs.SetFloat("CheckpointTime", 0f);
        totalRunTime = 0f;

        // Reset variables
        currentLevel = 1;
        checkpointLevel = 1;
        totalRunTime = 0f;

        isTimerRunning = false;
        isRunTimerRunning = false;

        Time.timeScale = 1f; // Just in case
        PlayerPrefs.SetFloat("CheckpointTime", 0f);
        totalRunTime = 0f;
        SceneManager.LoadScene("NumberSequence"); // Your scene name here
    }


    //Timer Start script
    void StartTimer()
    {
        currentRunTimeText.gameObject.SetActive(true); // show again
        if (PlayerPrefs.GetInt("EnableTimer", 1) == 0)
            return; // Timer is OFF

        timerText.gameObject.SetActive(true); // Show timer when countdown begins
        timeLeft = timePerLevel;
        isTimerRunning = true;

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();
    }

    void TimeOut()
    {
        isTimerRunning = false;
        allowClick = false;
        gameIsPaused = true;
        isRunTimerPaused = true; // Pause during Time's Up panel
        timerText.gameObject.SetActive(false);   //  Number mode

        if (timesUpPanel != null)
            timesUpPanel.SetActive(true);

        AudioManager.Instance.PlayLevelFailSound();
    }


    //Ui Manager
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

    public void LoadGameModeMenu()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("ModeSelector"); // Send me back to the Game Mode Selector

    }

    public void GoToHome()
    {
        AudioManager.Instance.PlayButtonSound();
        isTimerRunning = false;
        gameIsPaused = false; // add it after making changes
        allowClick = false;
        PlayerPrefs.SetFloat("CheckpointTime", 0f);
        totalRunTime = 0f;
        SceneManager.LoadScene("MainMenu"); //send to Home
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlayButtonSound();
        Time.timeScale = 2f;
        HideAllGamePanels();
        Debug.Log("Exit Pressed");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void HideAllGamePanels()
    {
        pauseMenuPanel.SetActive(false);
        timesUpPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameCompletedPanel.SetActive(false);
        //checkpointToastAnimator.gameObject.SetActive(false);
        pauseButton.SetActive(false);
    }

    //Checkpoint script

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



}
