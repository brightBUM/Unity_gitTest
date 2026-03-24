using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    void Update()
    {
        Move();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal"); 
        float v = Input.GetAxis("Vertical");   

        Vector3 move = new Vector3(h, 0, v);
        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}