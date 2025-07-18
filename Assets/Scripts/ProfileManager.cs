using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;

public class ProfileManager : MonoBehaviour
{
    public TMP_Text txtName;
    public TMP_InputField inputName;
    public TMP_Text txtError;

    public TMP_Text txtMemoryBest, txtMemoryCurrent;
    public TMP_Text txtNumberBest, txtNumberCurrent;

    private string memoryBoardId = "memory_speedrun";
    private string numberBoardId = "number_speedrun";

    public GameObject editPanel, profilePanel;

    public void ShowPanel()
    {
        AudioManager.Instance.PlayButtonSound();
        gameObject.SetActive(true);

        string currentName = AuthenticationService.Instance.PlayerName ?? PlayerPrefs.GetString("CachedPlayerName", "Guest");
        txtName.text = $"Name: {currentName}";
        inputName.text = currentName;
        txtError.gameObject.SetActive(false);

        // Show current times (local only)
        float memCurrent = PlayerPrefs.GetFloat("memory-current", -1);
        float numCurrent = PlayerPrefs.GetFloat("number-current", -1);

        txtMemoryCurrent.text = memCurrent < 0 ? "--" : $"{Mathf.RoundToInt(memCurrent)}s";
        txtNumberCurrent.text = numCurrent < 0 ? "--" : $"{Mathf.RoundToInt(numCurrent)}s";

        // Show best times from leaderboard (fallback to local best)
        _ = FetchBest(memoryBoardId, txtMemoryBest, "BestTime_Memory");
        _ = FetchBest(numberBoardId, txtNumberBest, "BestTime_Number");
    }

    public async Task FetchBest(string boardId, TMP_Text target, string fallbackKey)
    {
        try
        {
            var resp = await LeaderboardsService.Instance.GetPlayerScoreAsync(boardId);
            float best = resp != null ? (float)resp.Score : -1;
            if (best > 0)
                target.text = best.ToString("0.0") + "s";
            else
            {
                // fallback to PlayerPrefs
                float localBest = PlayerPrefs.GetFloat(fallbackKey, -1);
                target.text = localBest > 0 ? $"{localBest:0.0}s" : "--";
            }
        }
        catch
        {
            // Fallback on error
            float localBest = PlayerPrefs.GetFloat(fallbackKey, -1);
            target.text = localBest > 0 ? $"{localBest:0.0}s" : "--";
        }
    }

    public void OnClickEdit()
    {
        inputName.gameObject.SetActive(true);
        txtError.gameObject.SetActive(false);
    }

    public async void OnClickSave()
    {
        AudioManager.Instance.PlayButtonSound();
        string newName = inputName.text.Trim();
        if (string.IsNullOrEmpty(newName))
        {
            ShowError("Name cannot be empty");
            return;
        }

        // Check for duplicate
        var result = await LeaderboardsService.Instance.GetScoresAsync(memoryBoardId, new GetScoresOptions { Limit = 100 });
        foreach (var entry in result.Results)
        {
            if (entry.PlayerName == newName)
            {
                ShowError("Name already taken.");
                return;
            }
        }

        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            PlayerPrefs.SetString("CachedPlayerName", newName);
            PlayerPrefs.Save();
            txtName.text = $"Name: {newName}";
        }
        catch (System.Exception e)
        {
            ShowError($"Failed to update name: {e.Message}");
            return;
        }

        editPanel.SetActive(false);
        profilePanel.SetActive(true);
    }

    void ShowError(string message)
    {
        txtError.text = message;
        txtError.color = Color.red;
    }

    public void OnClickBackFromEdit()
    {
        AudioManager.Instance.PlayButtonSound();
        editPanel.SetActive(false);
        profilePanel.SetActive(true);
    }

    public void OnClickBackToMenu()
    {
        AudioManager.Instance.PlayButtonSound();
        gameObject.SetActive(false);
    }
}

