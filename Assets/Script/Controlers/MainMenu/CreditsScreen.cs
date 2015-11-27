using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditsScreen : MonoBehaviour {

    public delegate void voidDelegate();
    public event voidDelegate onClose;

    RectTransform RectTrans;
    public ScrollRect ScrollRec;
    Vector2 trueStart;

    public float ScrollDuration = 1f;
    float ScrollTime = 0f;
    bool scroll;
    
    void Start()
    {
        gameObject.SetActive(false);
        RectTrans = (RectTransform)transform;
        trueStart = RectTrans.anchoredPosition;
    }

    void OnEnable()
    {
        ScrollTime = 0;
        scroll = false;
    }

    void Update()
    {
        if (scroll && ScrollTime < ScrollDuration)
        {
            ScrollTime += Time.deltaTime;
        }


        ScrollRec.verticalNormalizedPosition = 1f- (ScrollTime / ScrollDuration) ;
    }

    public void CloseMenu()
    {
        if (onClose != null)
            onClose();

        if (gameObject.activeSelf)
            StartCoroutine(moveMain(-1, 4, changeStateToDisabledAtEnd: true));
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);

        StartCoroutine(moveMain(1, 10, true));
    }

    IEnumerator moveMain(int dir, float duration, bool startAtEnd = false, bool changeStateToDisabledAtEnd = false)
    {
        float l = RectTrans.rect.height * RectTrans.localScale.y;

        float endPoint = 0;
        if (dir > 0)
            endPoint = trueStart.y;
        else if (dir < 0)
            endPoint = trueStart.y - l;

        if (startAtEnd && dir > 0)
            RectTrans.anchoredPosition = new Vector2(RectTrans.anchoredPosition.x, trueStart.y - l);
        else if (startAtEnd && dir < 0)
            RectTrans.anchoredPosition = trueStart;
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
        else if (dir < 0)
        {
            while (RectTrans.anchoredPosition.y - endPoint > 0.5)
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

        if (changeStateToDisabledAtEnd)
            gameObject.SetActive(false);
        scroll = true;
        // Debug.Log("end option menu move");
    }
    bool stopMenuMove;
}
