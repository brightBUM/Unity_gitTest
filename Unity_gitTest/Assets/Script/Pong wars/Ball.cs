using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Settings")]
    public bool isBlackBall = true;
    public float speed = 3f;

    [Header("Audio")]
    public AudioClip wallBounceClip;
    public AudioClip blockDestroyClip;

    private Vector2 direction;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private Camera cam;
    private float camHeight, camWidth;
    private bool isProcessingHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = cam.orthographicSize * cam.aspect;

        Vector2[] directions =
        {
            new Vector2(1, 1),
            new Vector2(1, -1),
            new Vector2(-1, 1),
            new Vector2(-1, -1)
        };

        direction = directions[Random.Range(0, directions.Length)].normalized;
        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
            direction = rb.linearVelocity.normalized;

        ReflectOffEdges();
    }

    void ReflectOffEdges()
    {
        Vector3 pos = transform.position;
        float radius = circleCollider.radius * transform.localScale.x;
        bool reflected = false;

        if (pos.x - radius <= -camWidth)
        {
            direction.x = Mathf.Abs(direction.x);
            pos.x = -camWidth + radius;
            reflected = true;
        }
        else if (pos.x + radius >= camWidth)
        {
            direction.x = -Mathf.Abs(direction.x);
            pos.x = camWidth - radius;
            reflected = true;
        }

        if (pos.y - radius <= -camHeight)
        {
            direction.y = Mathf.Abs(direction.y);
            pos.y = -camHeight + radius;
            reflected = true;
        }
        else if (pos.y + radius >= camHeight)
        {
            direction.y = -Mathf.Abs(direction.y);
            pos.y = camHeight - radius;
            reflected = true;
        }

        if (reflected)
        {
            transform.position = pos;
            direction = NudgeDirection(direction);
            rb.linearVelocity = direction * speed;
            //SoundManager.instance.PlayWall(wallBounceClip);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isProcessingHit) return;

        int blackLayer = LayerMask.NameToLayer("Black");
        int whiteLayer = LayerMask.NameToLayer("White");
        int hitLayer = collision.gameObject.layer;

        bool shouldInteract = (isBlackBall && hitLayer == blackLayer) ||
                              (!isBlackBall && hitLayer == whiteLayer);

        if (!shouldInteract) return;

        isProcessingHit = true;

        Collider2D ballCollider = GetComponent<Collider2D>();
        Vector3 centre = collision.bounds.center;
        Vector3 hitPoint = ballCollider.ClosestPoint(centre);
        Vector3 diff = hitPoint - centre;

        diff.x /= collision.bounds.size.x;
        diff.y /= collision.bounds.size.y;

        float absX = Mathf.Abs(diff.x);
        float absY = Mathf.Abs(diff.y);
        float threshold = 0.25f;

        if (Mathf.Abs(absX - absY) < threshold)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                direction.x = -direction.x;
            else
                direction.y = -direction.y;
        }
        else if (absX > absY)
        {
            direction.x = -direction.x;
        }
        else
        {
            direction.y = -direction.y;
        }

        direction = NudgeDirection(direction);
        rb.linearVelocity = direction * speed;

        SoundManager.instance.PlayBlock(blockDestroyClip);

        GridSpawner.instance.SwapCell(collision.gameObject,
                                      collision.transform.position,
                                      isBlackBall);

        StartCoroutine(ResetHitLock());
    }

    Vector2 NudgeDirection(Vector2 dir)
    {
        float nudge = Random.Range(-0.15f, 0.15f);
        dir.x += nudge;
        dir.y += nudge;

        float minComponent = 0.3f;
        if (Mathf.Abs(dir.x) < minComponent)
            dir.x = minComponent * Mathf.Sign(dir.x);
        if (Mathf.Abs(dir.y) < minComponent)
            dir.y = minComponent * Mathf.Sign(dir.y);

        return dir.normalized;
    }

    IEnumerator ResetHitLock()
    {
        yield return null;
        isProcessingHit = false;
    }
}