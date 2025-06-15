using UnityEngine;
using UnityEngine.UI;

public class AdaptiveGrid : MonoBehaviour
{
    public GameObject gridPanel;  // Drag GridPanel here in the Inspector

    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    public int columns = 3;
    public int rows = 3;

    void Awake()
    {
        if (gridPanel == null)
        {
            Debug.LogError("GridPanel reference is missing in the Inspector.");
            return;
        }

        grid = gridPanel.GetComponent<GridLayoutGroup>();
        rectTransform = gridPanel.GetComponent<RectTransform>();

        if (grid == null || rectTransform == null)
        {
            Debug.LogError("GridLayoutGroup or RectTransform is missing on GridPanel.");
        }
    }

    void Start()
    {
        ResizeTiles();
    }

    public void ResizeTiles()
    {
        if (grid == null || rectTransform == null) return;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float cellWidth = (width - grid.padding.left - grid.padding.right - (grid.spacing.x * (columns - 1))) / columns;
        float cellHeight = (height - grid.padding.top - grid.padding.bottom - (grid.spacing.y * (rows - 1))) / rows;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }

    //  This lets you update grid size dynamically (e.g., next level)
    public void UpdateGridSize(int newSize)
    {
        columns = newSize;
        rows = newSize;
        ResizeTiles();
    }
}
