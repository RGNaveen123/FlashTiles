using UnityEngine;
using UnityEngine.SceneManagement;


public class StartMenuManager : MonoBehaviour
{

    public GameObject SettingsPanel;

    void Start()
    {
        AudioManager.Instance.PlayBGM(); //  Start music if not already playing
    }

    public void OnPlayClicked()
    {
        AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("ModeSelector");
    }

    public void OnExitClicked()
    {
        AudioManager.Instance.PlayButtonSound();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowSettings()
    {
        SettingsPanel.SetActive(true);
        AudioManager.Instance.PlayButtonSound();
    }

    public void HideSettings()
    {
        SettingsPanel.SetActive(false);
        AudioManager.Instance.PlayButtonSound();
    }
}
