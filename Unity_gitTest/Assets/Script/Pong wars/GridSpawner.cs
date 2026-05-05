using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public static GridSpawner instance;

    [Header("Prefabs")]
    public GameObject blackPrefab;
    public GameObject whitePrefab;

    [Header("Grid Settings")]
    public float cellSize = 0.5f;

    private Camera cam;
    private float camHeight, camWidth;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = cam.orthographicSize * cam.aspect;

        SpawnGrid();
    }

    void SpawnGrid()
    {
        float startX = -camWidth + cellSize / 2f;
        float startY = -camHeight + cellSize / 2f;

        int cols = Mathf.RoundToInt((camWidth * 2f) / cellSize);
        int rows = Mathf.RoundToInt((camHeight * 2f) / cellSize);

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                float x = startX + col * cellSize;
                float y = startY + row * cellSize;
                Vector3 pos = new Vector3(x, y, 0f);

                bool isLeftHalf = x < 0f;
                GameObject prefab = isLeftHalf ? blackPrefab : whitePrefab;
                string layerName = isLeftHalf ? "Black" : "White";

                GameObject cell = Instantiate(prefab, pos, Quaternion.identity, transform);
                cell.layer = LayerMask.NameToLayer(layerName);

                BoxCollider2D bc = cell.GetComponent<BoxCollider2D>();
                if (bc != null)
                {
                    bc.size = Vector2.one * cellSize;
                    bc.isTrigger = true;
                }
            }
        }
    }

    // Called by balls to swap a cell
    public void SwapCell(GameObject cell, Vector3 pos, bool destroyedByBlackBall)
    {
        // Black ball destroys black ? spawns white, and vice versa
        string newLayer = destroyedByBlackBall ? "White" : "Black";
        GameObject newPrefab = destroyedByBlackBall ? whitePrefab : blackPrefab;

        Destroy(cell);
        GameObject newCell = Instantiate(newPrefab, pos, Quaternion.identity, transform);
        newCell.layer = LayerMask.NameToLayer(newLayer);

        BoxCollider2D bc = newCell.GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            bc.size = Vector2.one * cellSize;
            bc.isTrigger = true;
        }
    }
}