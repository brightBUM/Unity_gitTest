using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool isBlackPowerUp; // matches the ball that should collect it
    public Transform ballsUI;
    void Start()
    {
        // Optional: add a gentle rotation for visibility
        StartCoroutine(Rotate());
    }

    System.Collections.IEnumerator Rotate()
    {
        while (true)
        {
            ballsUI.transform.Rotate(0f, 0f, 90f * Time.deltaTime);
            yield return null;
        }
    }
}