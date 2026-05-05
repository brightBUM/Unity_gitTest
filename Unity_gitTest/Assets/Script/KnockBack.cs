using UnityEngine;

public class KnockBack : MonoBehaviour, IDamageable
{
    public float force = 5f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage, Vector3 hitDirection)
    {
        rb.AddForce(hitDirection * force, ForceMode.Impulse);
    }
}