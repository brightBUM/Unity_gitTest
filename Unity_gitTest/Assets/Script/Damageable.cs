using NUnit.Framework;
using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
    public GameObject[] Hp; // Array
    int currentHP;

    void Start()
    {
        currentHP = Hp.Length;
    }

    public void TakeDamage(int damage, Vector3 hitDirection)
    {
        currentHP -= damage;

        if (currentHP >= 0 && currentHP < Hp.Length)
        {
            Hp[currentHP].SetActive(false);
        }

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}