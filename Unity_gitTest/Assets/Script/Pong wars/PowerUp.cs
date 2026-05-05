using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool isBlackPowerUp; // matches the ball that should collect it
    public Transform ballsUI;
    private void Update()
    {
        ballsUI.transform.Rotate(0f, 0f, 90f * Time.deltaTime);
    }
}