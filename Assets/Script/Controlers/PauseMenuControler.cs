using UnityEngine;
using System.Collections;

public class PauseMenuControler : MonoBehaviour
{

    [SerializeField]
    RectTransform RectTrans;

    [SerializeField]
    GameObject continueButton;

    [SerializeField]
    UnityEngine.UI.Image fadeImage;

    public void OnLevelWasLoaded(int level)
    {
        if (level == 1 || level == 2 || level == 3)
            gameObject.SetActive(false);
        else
            SetState(false);
    }

    public void SetState(bool b)
    {
        if (b)
        {
            RectTrans.gameObject.SetActive(true);
            fadeImage.gameObject.SetActive(true);
            continueButton.SetActive(true);
            StartCoroutine(moveMain(1, 10, true, false));
            StartCoroutine(FadeBackImage(true, 1.5f));
        }
        else
        {
            continueButton.SetActive(false);
            StartCoroutine(moveMain(-1, 1, changeStateToDisabledAtEnd: true));
            StartCoroutine(FadeBackImage(false, 1f));
        }
    }

    public void OpenOptions()
    {
        if (GameManager.optionsMenu)
            GameManager.optionsMenu.onClose += OptionsMenu_onClose;
        GameManager.optionsMenu.OpenMenu();

        StartCoroutine(moveMain(-1, 10));
        //gameObject.SetActive(false);
    }

    private void OptionsMenu_onClose()
    {
        openPause();
        GameManager.optionsMenu.onClose -= OptionsMenu_onClose;
    }

    void openPause()
    {
        //gameObject.SetActive(true);
        StartCoroutine(moveMain(1, 20));
    }

    public void OnDestroy()
    {

    }


    /// <summary>
    /// Moves main menu
    /// </summary>
    /// <param name="dir">move direction right = 1 left = -1</param>
    /// <param name="duration">the distance it will cover each second</param>
    /// <returns></returns>
    IEnumerator moveMain(int dir, float duration, bool startAtEnd = false, bool changeStateToDisabledAtEnd = false)
    {
        float l = RectTrans.rect.height * RectTrans.localScale.y;

        float endPoint = 0;
        if (dir > 0)
            endPoint = 0;
        else if (dir < 0)
            endPoint = l;

        if (startAtEnd && dir > 0)
            RectTrans.anchoredPosition = new Vector2(RectTrans.anchoredPosition.x, l);
        else if (startAtEnd && dir < 0)
            RectTrans.anchoredPosition = new Vector2(RectTrans.anchoredPosition.x, 0);
        Vector2 start, end;
        start = RectTrans.anchoredPosition;
        end = new Vector2(start.x, endPoint);

        float t = 0;

        stopMenuMove = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        stopMenuMove = false;

        if (dir > 0)
        {
            while (RectTrans.anchoredPosition.y - endPoint > 0.05)
            {
                //Debug.Log(" POS : " + (RectTrans.anchoredPosition.y - endPoint) + " DIR : " + dir);
                t += Time.deltaTime / duration;
                start = RectTrans.anchoredPosition;
                RectTrans.anchoredPosition = Vector2.Lerp(start, end, t);

                if (stopMenuMove)
                    break;

                yield return new WaitForEndOfFrame();
            }
        }
        else if (dir < 0)
        {
            while (RectTrans.anchoredPosition.y - endPoint < -10)
            {

                //Debug.Log(" POS : " + (RectTrans.anchoredPosition.y - endPoint) + " DIR : " + dir);
                t += Time.deltaTime / duration;
                start = RectTrans.anchoredPosition;
                RectTrans.anchoredPosition = Vector2.Lerp(start, end, t);

                if (stopMenuMove)
                    break;

                yield return new WaitForEndOfFrame();
            }
        }

        if (changeStateToDisabledAtEnd && dir < 0)
            RectTrans.gameObject.SetActive(false);
    }
    bool stopMenuMove;

    IEnumerator FadeBackImage(bool visable, float duration)
    {
        Color c1 = fadeImage.color;
        Color c2 = c1;

        c1.a = 0;
        c2.a = 1;
        float t = 0;

        if (!visable)
        {
            if (!fadeImage.isActiveAndEnabled)
                yield break;

            t = duration;
        }

        stopFading = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        stopFading = false;

        if (visable)
        {
            while(t<1)
            {
                t += Time.deltaTime / duration;

                fadeImage.color = Color.Lerp(c1, c2, t);

                if (stopFading)
                    break;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (t > 0)
            {
                t -= Time.deltaTime / duration;

                fadeImage.color = Color.Lerp(c1, c2, t);

                if (stopFading)
                    break;
                yield return new WaitForEndOfFrame();
            }
        }

        if (!visable)
            fadeImage.gameObject.SetActive(false);
    }
    bool stopFading;
}
