using UnityEngine;
using System.Collections;

public class specialSpriteSwapper : MonoBehaviour
{
    public float displayTime;
    public Sprite[] sprites;
    public SpriteRenderer r1, r2;
    float t1, t2;
    // Use this for initialization
    void Start()
    {
        GameObject g = new GameObject("Render1");
        MoveBoxScaler m;
        m = g.AddComponent<MoveBoxScaler>();
        m.StartSize = new Vector2(2048, 1625);
        m.defaultMode();

        r1 = g.AddComponent<SpriteRenderer>();

        g = new GameObject("Render2");
        m = g.AddComponent<MoveBoxScaler>();
        m.StartSize = new Vector2(2048, 1625);
        m.defaultMode();

        r2 = g.AddComponent<SpriteRenderer>();
        r2.sprite = sprites[Random.Range(0, sprites.Length)];

        t1 = displayTime;
        t2 = displayTime * 2;
    }


    Sprite p;
    // Update is called once per frame
    void Update()
    {
        if(t1>=displayTime*2f)
        {
            t1 = 0;
            p = sprites[Random.Range(0, sprites.Length)];
            while(p==r2.sprite)
                p = sprites[Random.Range(0, sprites.Length)];

            r1.sprite = p;
        }

        if(t2 >= displayTime * 2f)
        {
            t2 = 0;
            p = sprites[Random.Range(0, sprites.Length)];
            while (p == r1.sprite)
                p = sprites[Random.Range(0, sprites.Length)];

            r2.sprite = p;
        }

        if (t1 <= displayTime)
        {
            r1.color = Color.Lerp(Color.clear, Color.white, t1 / displayTime);
        }
        else if(t1 >= displayTime)
        {
            r1.color = Color.Lerp(Color.white, Color.clear, (t1 - displayTime) / displayTime);
        }

        if (t2 <= displayTime)
        {
            r2.color = Color.Lerp(Color.clear, Color.white, t2 / displayTime);
        }
        else if (t2 >= displayTime)
        {
            r2.color = Color.Lerp(Color.white, Color.clear, (t2 - displayTime) / displayTime);
        }

        t1 += Time.deltaTime;
        t2 += Time.deltaTime;
    }
}
