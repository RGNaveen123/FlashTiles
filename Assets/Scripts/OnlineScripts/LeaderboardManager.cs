using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
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
    public void ShowMemory() => _ = RefreshAndShow(memoryBoardId, memoryContent);
    public void ShowNumber() => _ = RefreshAndShow(numberBoardId, numberContent);
    public void HidePage() => leaderboardPage.SetActive(false);



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

    /* -------- internals -------- */
    async Task RefreshAndShow(string boardId, Transform contentRoot)
    {
        leaderboardPage.SetActive(true);
        Debug.Log("contentRoot = " + contentRoot.name);

        // clear old rows
        //foreach (Transform c in contentRoot) Destroy(c.gameObject);
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(contentRoot.GetChild(i).gameObject);
        }

            try
        {
            var resp = await LeaderboardsService.Instance.GetScoresAsync(
                            boardId,
                            new GetScoresOptions { Limit = 20 });

            foreach (var entry in resp.Results)
            {
                var row = Instantiate(rowPrefab, contentRoot);
                row.Set(entry.PlayerName, (float)(entry.Score / 1000.0));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LB fetch failed: {e}");
        }
    }


}
