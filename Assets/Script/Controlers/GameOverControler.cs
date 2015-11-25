using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using util;

public class GameOverControler : MonoBehaviour {

    [System.Serializable]
    public struct Scoreboard
    {
        public Text numb0, numb1, numb2;

        public void setValue(int i)
        {
            numb0.text = i.getNumbAt(0).ToString();
            numb1.text = i.getNumbAt(1).ToString();
            numb2.text = i.getNumbAt(2).ToString();
        }
    }

    public Scoreboard CurrentScore, BestScore;

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

        float duration = 4f;
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

        while (timer <= duration && Vector2.Distance(StartPos, Endpos) > 0.45f) 
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

        StartCoroutine(NumberIntro());

        #region flickering
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
        #endregion

        

        foreach (Image i in flickerImages)
        {
            c = i.color;
            c.a = 1;

            i.color = c;
        }
    }

    IEnumerator NumberIntro()
    {
        int score = 123;
        int scoreBest = 453;

        int x = 0;

        for (int i = 0; i < 30; i++)
        {
            if (i % 3 == 0)
                x++;

            CurrentScore.numb0.text = Random.Range(0, 9).ToString();
            if (x > 1)
                BestScore.numb0.text = Random.Range(0, 9).ToString();
            if (x > 5) 
                CurrentScore.numb1.text = Random.Range(0, 9).ToString();
            if(x>6)
                BestScore.numb1.text = Random.Range(0, 9).ToString();
            if(x>10)
                CurrentScore.numb2.text = Random.Range(0, 9).ToString();
            if(x>16)
                BestScore.numb2.text = Random.Range(0, 9).ToString();

            yield return new WaitForSeconds(0.05f);
        }

        x = 20;
        for (int i = 0; i < 60; i++)
        {
            if (i % 3 == 0)
                x--;

            if (x > 15)
                CurrentScore.numb0.text = Random.Range(0, 9).ToString();
            else
                CurrentScore.numb0.text = score.getNumbAt(0).ToString();

            if (x > 14)
                BestScore.numb0.text = Random.Range(0, 9).ToString();
            else
                BestScore.numb0.text = scoreBest.getNumbAt(0).ToString();

            if (x > 10)
                CurrentScore.numb1.text = Random.Range(0, 9).ToString();
            else
                CurrentScore.numb1.text = score.getNumbAt(1).ToString();

            if (x > 9)
                BestScore.numb1.text = Random.Range(0, 9).ToString();
            else
                BestScore.numb1.text = scoreBest.getNumbAt(1).ToString();

            if (x > 5)
                CurrentScore.numb2.text = Random.Range(0, 9).ToString();
            else
                CurrentScore.numb2.text = score.getNumbAt(2).ToString();

            if (x > 4)
                BestScore.numb2.text = Random.Range(0, 9).ToString();
            else
                BestScore.numb2.text = scoreBest.getNumbAt(2).ToString();

            yield return new WaitForSeconds(0.05f);
        }

        BestScore.setValue(scoreBest);
        CurrentScore.setValue(score);
    }

    [ContextMenu("ReDo animation")]
    public void Reload()
    {
        if (Application.isPlaying)
            StartCoroutine(IntroAnimation());
    }

    [ContextMenu("Reload Numbers")]
    public void ReloadNumb()
    {
        if (Application.isPlaying)
            StartCoroutine(NumberIntro());
    }
}
