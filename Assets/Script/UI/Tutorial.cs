using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour
{

    public delegate void voidDelegate();
    public event voidDelegate onClose;

    public Sprite[] Screens;
    public Image DisplayObject;
    int Index;

    public RectTransform Container;

    public void Next()
    {
        if (Index + 1 < Screens.Length)
        {
            Index++;
            DisplayObject.sprite = Screens[Index];
        }
    }

    public void Previous()
    {
        if (Index - 1 >= 0)
        {
            Index--;
            DisplayObject.sprite = Screens[Index];
        }
    }

    public void Close()
    {
        if (onClose != null)
            onClose();

        if (gameObject.activeSelf)
            StartCoroutine(moveMain(-1, 4, changeStateToDisabledAtEnd: true));
    }

    public void Open()
    {
        gameObject.SetActive(true);

        StartCoroutine(moveMain(1, 10, true));
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Moves main menu
    /// </summary>
    /// <param name="dir">move direction right = 1 left = -1</param>
    /// <param name="duration">the distance it will cover each second</param>
    /// <returns></returns>
    IEnumerator moveMain(int dir, float duration, bool startAtEnd = false, bool changeStateToDisabledAtEnd = false)
    {
        float l = Container.rect.height * Container.localScale.y;

        float endPoint = 0;
        if (dir > 0)
            endPoint = 0;
        else if (dir < 0)
            endPoint = -l;

        if (startAtEnd && dir > 0)
            Container.anchoredPosition = new Vector2(Container.anchoredPosition.x, -l);
        else if (startAtEnd && dir < 0)
            Container.anchoredPosition = new Vector2(Container.anchoredPosition.x, 0);
        Vector2 start, end;
        start = Container.anchoredPosition;
        end = new Vector2(start.x, endPoint);

        float t = 0;

        stopMenuMove = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        stopMenuMove = false;

        if (dir > 0)
        {
            while (Container.anchoredPosition.y + endPoint < -10)
            {
                //  Debug.Log(" POS : " + (RectTrans.anchoredPosition.y - endPoint) + " DIR : " + dir);

                t += Time.deltaTime / duration;
                start = Container.anchoredPosition;
                Container.anchoredPosition = Vector2.Lerp(start, end, t);

                if (stopMenuMove)
                    break;

                yield return new WaitForEndOfFrame();
            }
        }
        else if (dir < 0)
        {
            while (Container.anchoredPosition.y - endPoint > 0.5)
            {

                // Debug.Log(" POS : " + (RectTrans.anchoredPosition.y - endPoint) + " DIR : " + dir);

                t += Time.deltaTime / duration;
                start = Container.anchoredPosition;
                Container.anchoredPosition = Vector2.Lerp(start, end, t);

                if (stopMenuMove)
                    break;

                yield return new WaitForEndOfFrame();
            }
        }

        if (changeStateToDisabledAtEnd)
            gameObject.SetActive(false);

        // Debug.Log("end option menu move");
    }
    bool stopMenuMove;
}
