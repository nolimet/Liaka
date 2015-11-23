using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeIn : MonoBehaviour
{


    public float fadeDuration = 1f;
    float timer;
    SpriteRenderer img;
    Color c1,c2;

    [SerializeField]
    GameObject StageManager;

    // Use this for initialization
    void Start()
    {
        timer = fadeDuration;
        img = GetComponent<SpriteRenderer>();
        c1 = c2 = img.color;
        c2.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer-=Time.deltaTime;
        img.color = Color.Lerp(c2, c1, timer / fadeDuration);

        if (timer > 0)
            StageManager.SetActive(false);
        else
        {
            StageManager.SetActive(true);
            Destroy(gameObject);
        }
    }
}