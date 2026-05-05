using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Controls : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    Rigidbody rb;
    Animator anim;
    float horizontal;
    float vertical;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 lookpos = hit.point;
            lookpos.y = transform.position.y;
            transform.LookAt(lookpos);

            Debug.DrawLine(transform.position, hit.point, Color.red);

        }

        float movementAmount = new Vector2(horizontal, vertical).magnitude;

        if (movementAmount == 0)
        {
            anim.SetFloat("Speed", 0); // Idle
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetFloat("Speed", 1); // Run
        }
        else
        {
            anim.SetFloat("Speed", 0.5f); // Walk
        }

    }

    private void FixedUpdate()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Spin_Atk") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Sd_Atk"))
                return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = isRunning ? speed : speed * 0.3f;

        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        rb.MovePosition(transform.position + move * currentSpeed * Time.fixedDeltaTime);

    }
}
