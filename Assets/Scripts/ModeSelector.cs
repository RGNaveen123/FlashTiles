using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelector : MonoBehaviour
{
    public GameObject SettingsPanel;
    public void LoadMemoryGame()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("MemoryGameScene");
        Time.timeScale = 1f;
    }

    public void LoadNumberSequence()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("NumberSequence");
        Time.timeScale = 1f;
    }

    public void BackToMainMenu()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenGameSettings()
    {
        AudioManager.Instance.PlayButtonSound();
        SettingsPanel.SetActive(true);
    }
}
