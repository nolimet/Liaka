using UnityEngine;
using System.Collections;

public class SpriteSwapper : MonoBehaviour
{

    public Sprite[] Sprites;
    public void OnEnable()
    {
        GetComponent<SpriteRenderer>().sprite = Sprites[Random.Range(0, Sprites.Length)];
    }
}
