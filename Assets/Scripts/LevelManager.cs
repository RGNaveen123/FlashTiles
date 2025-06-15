using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int currentLevel = 1;
    public int maxLevel = 10;

    public AdaptiveGrid gridScript; // Attach in Inspector

    void Start()
    {
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int level)
    {
        // Set the grid size or logic depending on the level
        int gridSize = Mathf.Min(3 + (level - 1), 10); // 3x3 to 10x10
        gridScript.UpdateGridSize(gridSize);
    }

    public void OnLevelCompleted()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            LoadLevel(currentLevel);
        }
        else
        {
            Debug.Log(" All levels completed!");
            // Show end screen or restart
        }
    }
}
