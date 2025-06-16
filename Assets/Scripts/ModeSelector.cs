using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelector : MonoBehaviour
{
    public void LoadMemoryGame()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("MemoryGameScene");
        Time.timeScale = 1f;
    }

    public void LoadNumberSequence()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("SequenceMode");
        Time.timeScale = 1f;
    }

    public void BackToMainMenu()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("StartMenuScene");
    }
}
