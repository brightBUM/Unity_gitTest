using UnityEngine;

public class Block : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Convert(bool toBlack)
    {
        gameObject.layer = LayerMask.NameToLayer(toBlack ? "Black" : "White");
        sr.color = toBlack ? GridSpawner.instance.teamAColor : GridSpawner.instance.teamBColor;
    }
}