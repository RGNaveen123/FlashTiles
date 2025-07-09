using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text scoreText;

    public void Set(string playerName, float timeSec)
    {
        playerNameText.text = playerName;
        scoreText.text = $"{timeSec:F1}s";
    }
}
