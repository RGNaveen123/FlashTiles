//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class MainMenuManager : MonoBehaviour
//{
    
//    public MemoryGameManager GameManager;

//    public void Start()
//    {   
//        GameManager = gameObject.AddComponent<MemoryGameManager>();
//    }

//    public void StartGame()
//    {
//        //AudioManager.Instance.ButtonClickSound();
//        SceneManager.LoadScene("MemoryGameScene");  // Match the exact scene name
//        GameManager.RestartGame();
//    }

//    public void ExitGame()
//    {
//        Application.Quit();
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
//    }
//}
