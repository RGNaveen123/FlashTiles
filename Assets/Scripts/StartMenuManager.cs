using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
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
}
