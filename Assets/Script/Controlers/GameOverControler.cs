using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class GameOverControler : MonoBehaviour {

    struct Scoreboard
    {
        Text numb0, numb1, numb3;
    }

    public Image[] flickerImages;
    
	// Use this for initialization
	void Start () {
        StartCoroutine(IntroAnimation());
	}
	
	IEnumerator IntroAnimation()
    {
        RectTransform rect = (RectTransform)transform;

        Vector2 Endpos = rect.anchoredPosition;
        Vector2 StartPos = rect.anchoredPosition;

        float timer = 0f;
        float[] timer2 = new float[flickerImages.Length];

        float duration = 1.5f;
        float[] duration2 = new float[timer2.Length];

        bool[] direction = new bool[timer2.Length];

        int[] flicker = new int[timer2.Length];
        int l = timer2.Length;

        StartPos.x = StartPos.x + rect.rect.width;

        rect.anchoredPosition = StartPos;

        Color c = Color.white;
        c.a = 0;
        foreach(Image i in flickerImages)
        {
            c = i.color;
            c.a = 0;

            i.color = c;
        }

        while (timer <= duration)
        {
            rect.anchoredPosition = Vector2.Lerp(StartPos, Endpos, timer / duration);
            StartPos = rect.anchoredPosition;
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < l; i++)
        {
            duration2[i] = Random.Range(1.4f, 1.6f);
            flicker[i] = Random.Range(7, 9);
        }

        while (flicker.Any(a => a > 0))
        {
            for (int i = 0; i < l; i++)
            {
                if (flicker[i] > 0)
                {
                    if (direction[i])
                    {
                        if (timer2[i] <= duration2[i])
                        {
                            timer2[i] += Time.deltaTime;
                        }
                        else
                        {
                            direction[i] = false;
                        }
                    }
                    else
                    {
                        if (timer2[i] > 0)
                        {
                            timer2[i] -= Time.deltaTime;
                        }
                        else
                        {
                            direction[i] = true;
                            flicker[i]--;
                            duration2[i] *= 0.5f;
                        }
                    }

                    flickerImages[i].color = Color.Lerp(c, Color.white, timer2[i] / duration2[i]);
                }
            }

            yield return new WaitForEndOfFrame();
        }

        foreach (Image i in flickerImages)
        {
            c = i.color;
            c.a = 1;

            i.color = c;
        }

    }

    [ContextMenu("ReDo animation")]
    public void Reload()
    {
        if (Application.isPlaying)
            StartCoroutine(IntroAnimation());
    }
}
