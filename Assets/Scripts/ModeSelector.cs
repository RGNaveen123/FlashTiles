using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelector : MonoBehaviour
{
    public void LoadMemoryGame()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("MemoryGameScene");
    }

    public void BackToMainMenu()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("StartMenuScene");
    }
}
