using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;


public class LeaderboardManager : MonoBehaviour
{

    public static LeaderboardManager Instance { get; private set; }

    /* NEW: guard so we don’t re‑initialise */
    private static bool alreadyInitialised = false;

    async void Awake()
    {
        // --- standard singleton boiler‑plate ---
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- run initialisation only once ---
        if (alreadyInitialised) return;          // <<-- early‑out
        alreadyInitialised = true;

        await InitialiseUGSAsync();
    }

    /* -------- inspector hooks  -------- */
    [Header("UGS Board IDs")]
    public string memoryBoardId = "memory_speedrun";
    public string numberBoardId = "number_speedrun";

    [Header("UI")]
    public GameObject leaderboardPage;              // ← LeaderboardPage root
    public Transform memoryContent;                 // ← MemoryContent
    public Transform numberContent;                 // ← NumberContent
    public LeaderboardRow rowPrefab;                // ← the prefab you built

    [Header("UI Controls")]
    public TextMeshProUGUI leaderboardTitleText;
    public TextMeshProUGUI toggleButtonText;
    public GameObject togglebutton;

    private bool showingMemoryBoard = true;

    // Switching function from memory to number game

    public void ToggleLeaderboard()
    {
        showingMemoryBoard = !showingMemoryBoard;
        AudioManager.Instance.PlayButtonSound();
        if (showingMemoryBoard)
        {
            numberContent.gameObject.SetActive(false);
            memoryContent.gameObject.SetActive(true);

            leaderboardTitleText.text = "Memory Leaderboard";
            toggleButtonText.text = "Switch to Number Leaderboard";

            _ = RefreshAndShow(memoryBoardId, memoryContent);
        }
        else
        {
            memoryContent.gameObject.SetActive(false);
            numberContent.gameObject.SetActive(true);

            leaderboardTitleText.text = "Number Leaderboard";
            toggleButtonText.text = "Switch to Memory Leaderboard";

            _ = RefreshAndShow(numberBoardId, numberContent);
        }
    }


    /* split the async logic out for clarity */
    private async Task InitialiseUGSAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            Debug.Log($"UGS ready. PlayerId = {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError("UGS init failed: " + e.Message);
        }
    }

    /* ========================================= */

    #region PUBLIC API
    public void ShowMemory()
    {
        AudioManager.Instance.PlayButtonSound();
        numberContent.gameObject.SetActive(false);   // hide number leaderboard
        memoryContent.gameObject.SetActive(true);    // show memory leaderboard
        leaderboardPage.SetActive(true);

        _ = RefreshAndShow(memoryBoardId, memoryContent);
    }

    public void ShowNumber()
    {
        AudioManager.Instance.PlayButtonSound();
        memoryContent.gameObject.SetActive(false);   // hide memory leaderboard
        numberContent.gameObject.SetActive(true);    // show number leaderboard
        leaderboardPage.SetActive(true);

        _ = RefreshAndShow(numberBoardId, numberContent);
    }

    public void HidePage()
    {
        AudioManager.Instance.PlayButtonSound();
        memoryContent.gameObject.SetActive(false);
        numberContent.gameObject.SetActive(false);
        leaderboardPage.SetActive(false);
    }
        



    /* ---------- new SubmitScore ---------- */
    public async void SubmitScore(string boardId, int scoreSeconds)
    {
        try
        {
            var opts = new AddPlayerScoreOptions
            {
                // attach the player‑chosen name that you already store in Authentication
                Metadata = new Dictionary<string, string>
                {
                    { "playerName", AuthenticationService.Instance.PlayerName }   // optional but handy
                }
            };

            /* 2. Await the SDK call */
            await LeaderboardsService.Instance.AddPlayerScoreAsync(boardId, scoreSeconds, opts);
        }
        catch (Exception ex)
        {
            Debug.LogError("Leaderboard submit failed: " + ex.Message);
        }
    }
    #endregion

    async Task RefreshAndShow(string boardId, Transform contentRoot)
    {
        leaderboardPage.SetActive(true);

        // Clear previous entries
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }

        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                boardId,
                new GetScoresOptions { Limit = 10 }  // Top 10
            );

            var results = scoresResponse.Results;

            for (int i = 0; i < results.Count; i++)
            {
                var entry = results[i];

                // Create a row instance
                var row = Instantiate(rowPrefab, contentRoot);

                // Use entry.PlayerName (if set), fallback to PlayerId
                string displayName = !string.IsNullOrEmpty(entry.PlayerName)
                    ? entry.PlayerName
                    : $"Player {entry.Rank}";

                float seconds = (float)entry.Score;

                // Set the row (with rank starting from 1)
                row.Set(i + 1, displayName, seconds);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Leaderboard fetch failed: " + ex.Message);
        }
    }



    // Development purpose Script

    public void ResetBestTimes()
    {
        PlayerPrefs.DeleteKey("BestTime_Memory");
        PlayerPrefs.DeleteKey("BestTime_Number");
        PlayerPrefs.Save();

        Debug.Log(" Best time scores have been reset.");
    }



}
