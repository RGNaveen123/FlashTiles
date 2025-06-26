using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;

public class LeaderboardManager : MonoBehaviour
{
    public string leaderboardId = "SpeedRun"; // Replace with your actual leaderboard ID

    private async void Start()
    {
        // Ensure Unity Services are initialized
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void SubmitScore(int score)
    {
        try
        {
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            Debug.Log($"Score submitted: {score}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to submit score: " + e.Message);
        }
    }
}
