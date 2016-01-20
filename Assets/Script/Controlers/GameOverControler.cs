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
        public Text[] numbs;

        int l;

        public void setValue(int i)
        {
            l = numbs.Length;
            for (int j = 0; j < l; j++)
            {
                numbs[j].text = i.getNumbAt(j).ToString();
            }
           // numb.text = i.getNumbAt(0).ToString();
           // numb1.text = i.getNumbAt(1).ToString();
           // numb2.text = i.getNumbAt(2).ToString();
        }

        /// <summary>
        /// Set Values of TextObjects;
        /// </summary>
        /// <param name="s"></param>
        public void setValue(string s)
        {
            l = numbs.Length;
            for (int i = 0; i < l && i < s.Length; i++) 
            {
                numbs[i].text = s[i].ToString();
            }
        }

        public void SetRandomNumber(int AffectedNumb)
        {
            l = numbs.Length;
            for (int i = 0; i < l && i < AffectedNumb; i++)
            {
                numbs[i].text = Random.Range(0, 10).ToString();
            }
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

        StartCoroutine(NumberIntro());

        foreach (Image i in flickerImages)
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
        //int score = GameManager.instance.coinsCollectedRun;
        int score = GameManager.instance.coinsCollectedRun;
        //int scoreBest = GameManager.instance.saveDat.game.HighestAmountGainedInSingleRun;
        int scoreBest = GameManager.instance.saveDat.game.HighestAmountGainedInSingleRun;


        //yield return new WaitForSeconds(0.5f);

        int x = 0;
        for (int i = 0; i < 30; i++)
        {
            if (i % 3 == 0)
                x++;

            if(x > 10)
            {
                CurrentScore.SetRandomNumber(3);
                BestScore.SetRandomNumber(3);
            }
            else if (x > 5)
            {
                CurrentScore.SetRandomNumber(2);
                BestScore.SetRandomNumber(2);
            }

            else if(x>1)
            {
                CurrentScore.SetRandomNumber(1);
                BestScore.SetRandomNumber(1);
            }
            //CurrentScore.numb[0].text = Random.Range(0, 9).ToString();
            //if (x > 1)
            //    BestScore.numb[0].text = Random.Range(0, 9).ToString();
            //if (x > 2) 
            //    CurrentScore.numb1.text = Random.Range(0, 9).ToString();
            //if(x>3)
            //    BestScore.numb1.text = Random.Range(0, 9).ToString();
            //if(x>4)
            //    CurrentScore.numb2.text = Random.Range(0, 9).ToString();
            //if(x>5)
            //    BestScore.numb2.text = Random.Range(0, 9).ToString();

            yield return new WaitForSeconds(0.05f);
        }

        x = 20;
        string CScore = "000";
        string BScore = "000";
        for (int i = 0; i < 60; i++)
        {
            if (i % 3 == 0)
                x--;
            CScore = "";
            BScore = "";


            if (x > 15)
                CScore += Random.Range(0, 9).ToString();
            else
                CScore += score.getNumbAt(0).ToString();

            if (x > 14)
                BScore += Random.Range(0, 9).ToString();
            else
                BScore += scoreBest.getNumbAt(0).ToString();

            if (x > 10)
                CScore += Random.Range(0, 9).ToString();
            else
                CScore += score.getNumbAt(1).ToString();

            if (x > 9)
                BScore += Random.Range(0, 9).ToString();
            else
                BScore += scoreBest.getNumbAt(1).ToString();

            if (x > 5)
                CScore += Random.Range(0, 9).ToString();
            else
                CScore += score.getNumbAt(2).ToString();

            if (x > 4)
                BScore += Random.Range(0, 9).ToString();
            else
                BScore += scoreBest.getNumbAt(2).ToString();

            BestScore.setValue(BScore);
            CurrentScore.setValue(CScore);

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
