using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System.Collections.Generic;
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

    void Start()
    {
        string currentName = AuthenticationService.Instance.PlayerName ?? PlayerPrefs.GetString("CachedPlayerName", "Guest");
        txtName.text = $"Name: {currentName}";
    }


    void ShowTimes()
    {
        float memoryBest = PlayerPrefs.GetFloat("memory-best", -1);
        float memoryCurrent = PlayerPrefs.GetFloat("memory-current", -1);
        float numberBest = PlayerPrefs.GetFloat("number-best", -1);
        float numberCurrent = PlayerPrefs.GetFloat("number-current", -1);

        txtMemoryBest.text = memoryBest < 0 ? "--" : $"{memoryBest:0.0}s";
        txtMemoryCurrent.text = memoryCurrent < 0 ? "--" : $"{memoryCurrent:0.0}s";
        txtNumberBest.text = numberBest < 0 ? "--" : $"{numberBest:0.0}s";
        txtNumberCurrent.text = numberCurrent < 0 ? "--" : $"{numberCurrent:0.0}s";
    }



    public void ShowPanel()
    {
        AudioManager.Instance.PlayButtonSound();
        gameObject.SetActive(true);
        string currentName = AuthenticationService.Instance.PlayerName ?? PlayerPrefs.GetString("CachedPlayerName", "Guest");
        txtName.text = $"Name: {currentName}";
        inputName.text = currentName;
        txtError.gameObject.SetActive(false);

        txtMemoryCurrent.text = PlayerPrefs.GetFloat("memory-current", -1) < 0 ? "--" : PlayerPrefs.GetFloat("memory-current").ToString("0.0s");
        txtNumberCurrent.text = PlayerPrefs.GetFloat("number-current", -1) < 0 ? "--" : PlayerPrefs.GetFloat("number-current").ToString("0.0s");

        _ = FetchBest(memoryBoardId, txtMemoryBest);
        _ = FetchBest(numberBoardId, txtNumberBest);
    }


    public async Task FetchBest(string boardId, TMP_Text target)
    {
        var resp = await LeaderboardsService.Instance.GetPlayerScoreAsync(boardId);
        float best = (float)(resp?.Score ?? 0) / 1000f;
        target.text = best.ToString("0.0s");
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

        // Check duplicate
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

    async Task CheckDuplicateAndSave(string newName)
    {
        var result = await LeaderboardsService.Instance.GetScoresAsync(memoryBoardId, new GetScoresOptions { Limit = 100 });
        foreach (var entry in result.Results)
        {
            if (entry.PlayerName == newName)
            {
                ShowError("Name already taken.");
                return;
            }
        }

        // Save
        PlayerPrefs.SetString("playerName", newName);
        PlayerPrefs.Save();
        txtName.text = newName;
        inputName.gameObject.SetActive(false);
        Debug.Log("Name updated successfully");
    }

    //Back button

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
        // Add logic to show main menu if needed
    }


}
