using UnityEngine;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour
{
    public static GridSpawner instance;

    [Header("Team Colors")]
    public Color teamAColor = Color.black;
    public Color teamBColor = Color.white;

    [Header("Grid Settings")]
    public float cellSize = 0.5f;

    [Header("PowerUp Settings")]
    [Range(0f, 1f)]
    public float powerUpSpawnChance = 0.05f;

    private Camera cam;
    private float camHeight, camWidth;

    private HashSet<Block> pendingConversion = new HashSet<Block>();

    // Pool indices
    const int BLOCK = 0;
    const int POWERUP = 1;
    const int BLACK_BALL = 2;
    const int WHITE_BALL = 3;

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

    void LateUpdate()
    {
        pendingConversion.Clear();
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

                GameObject cell = ObjectPoolManager.Instance.Spawn(BLOCK, pos, Quaternion.identity);
                Block block = cell.GetComponent<Block>();
                block.Convert(x < 0f);
            }
        }
    }

    // Called by Ball when it hits a block
    public void ConvertCell(Block block, bool destroyedByBlackBall)
    {
        if (block == null || !block.gameObject.activeInHierarchy) return;
        if (pendingConversion.Contains(block)) return;

        pendingConversion.Add(block);

        // Black block → convert to white, white block → convert to black
        bool blockWasBlack = block.gameObject.layer == LayerMask.NameToLayer("Black");
        block.Convert(!blockWasBlack);

        // Roll for power up
        if (Random.value <= powerUpSpawnChance)
        {
            // Spawn in destroying ball's own area
            float minX = destroyedByBlackBall ? -camWidth : 0f;
            float maxX = destroyedByBlackBall ? 0f : camWidth;
            float randX = Random.Range(minX + cellSize, maxX - cellSize);
            float randY = Random.Range(-camHeight + cellSize, camHeight - cellSize);
            Vector3 powerUpPos = SnapToGrid(new Vector3(randX, randY, 0f));

            GameObject powerUp = ObjectPoolManager.Instance.Spawn(POWERUP, powerUpPos, Quaternion.identity);
            powerUp.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void ApplyBallColor(GameObject ball, bool isBlackBall)
    {
        SpriteRenderer sr = ball.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = isBlackBall ? teamAColor : teamBColor;
    }

    public GameObject SpawnBall(bool isBlackBall, Vector3 pos, Vector2 velocity)
    {
        int index = isBlackBall ? BLACK_BALL : WHITE_BALL;
        GameObject ball = ObjectPoolManager.Instance.Spawn(index, pos, Quaternion.identity);

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.linearVelocity = velocity;
        }

        Ball ballScript = ball.GetComponent<Ball>();
        if (ballScript != null)
        {
            ballScript.isBlackBall = isBlackBall;
            ballScript.InitBall();
        }

        ApplyBallColor(ball, isBlackBall);
        return ball;
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        float halfCell = cellSize / 2f;
        float x = Mathf.Round((pos.x - halfCell) / cellSize) * cellSize + halfCell;
        float y = Mathf.Round((pos.y - halfCell) / cellSize) * cellSize + halfCell;
        return new Vector3(x, y, 0f);
    }
}