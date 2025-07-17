using UnityEngine;
using TMPro;

public class LeaderboardRow : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text playerNameText;
    public TMP_Text scoreText;

    public void Set(int rank, string playerName, float seconds)
    {
        rankText.text = rank.ToString();
        playerNameText.text = playerName;
        scoreText.text = seconds.ToString("0.0") + "s";
    }
}
