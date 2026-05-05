using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public static GridSpawner instance;

    [Header("Prefabs")]
    public GameObject blackBallPrefab;
    public GameObject whiteBallPrefab;
    public GameObject blackPrefab;
    public GameObject whitePrefab;
    public GameObject powerUpPrefab;

    [Header("Grid Settings")]
    public float cellSize = 0.5f;

    [Header("PowerUp Settings")]
    [Range(0f, 1f)]
    public float powerUpSpawnChance = 0.05f;

    [Header("Team Colors")]
    public Color teamAColor = Color.black;
    public Color teamBColor = Color.white;

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
                SpawnCell(pos, isLeftHalf);
            }
        }
    }

    public void SpawnCell(Vector3 pos, bool isBlackSide)
    {
        GameObject prefab = isBlackSide ? blackPrefab : whitePrefab;
        string layerName = isBlackSide ? "Black" : "White";

        GameObject cell = Instantiate(prefab, pos, Quaternion.identity, transform);
        cell.layer = LayerMask.NameToLayer(layerName);

        // Apply team color to sprite
        SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = isBlackSide ? teamAColor : teamBColor;

        BoxCollider2D bc = cell.GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            bc.size = Vector2.one * cellSize;
            bc.isTrigger = true;
        }
    }

    public void SwapCell(GameObject cell, Vector3 pos, bool destroyedByBlackBall)
    {
        Destroy(cell);

        // Roll for power up
        if (Random.value <= powerUpSpawnChance)
        {
            GameObject powerUp = Instantiate(powerUpPrefab, pos, Quaternion.identity, transform);

            CircleCollider2D cc = powerUp.GetComponent<CircleCollider2D>();
            if (cc == null) cc = powerUp.AddComponent<CircleCollider2D>();
            cc.isTrigger = true;
            cc.radius = cellSize * 0.4f;
            return;
        }

        // Spawn regular swapped cell
        bool newSideIsBlack = !destroyedByBlackBall;
        string newLayer = newSideIsBlack ? "Black" : "White";
        GameObject newPrefab = newSideIsBlack ? blackPrefab : whitePrefab;

        GameObject newCell = Instantiate(newPrefab, pos, Quaternion.identity, transform);
        newCell.layer = LayerMask.NameToLayer(newLayer);

        SpriteRenderer sr = newCell.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = newSideIsBlack ? teamAColor : teamBColor;

        BoxCollider2D bc = newCell.GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            bc.size = Vector2.one * cellSize;
            bc.isTrigger = true;
        }
    }

    public void ApplyBallColor(GameObject ball, bool isBlackBall)
    {
        SpriteRenderer sr = ball.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = isBlackBall ? teamAColor : teamBColor;
    }
}