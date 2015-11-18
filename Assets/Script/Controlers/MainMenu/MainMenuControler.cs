﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuControler : MonoBehaviour {

    [System.Serializable]
    public struct opMenu
    {
        public Slider sounds, music,interfaceVol;
        public GameObject menu;
    }
    public opMenu OptionMenu;

    public Text CoinsDisplay;

    public RectTransform MainMenu;

    int updateDelay;

    void Start()
    {
        CoinsDisplay.text = "Current Gold:" + "\n " + GameManager.instance.saveDat.game.CoinsCurrent.ToString();

        MainMenu.anchoredPosition = MainMenu.anchoredPosition - new Vector2(MainMenu.rect.width, 0);
        StartCoroutine(moveMain(1, 20));

        if (GameManager.optionsMenu)
            GameManager.optionsMenu.CloseMenu();

    }

    public void Update()
    {
        updateDelay++;
        if (updateDelay > 20)
        {
            updateDelay = 0;

            CoinsDisplay.text = "Current Gold: " + GameManager.instance.saveDat.game.CoinsCurrent.ToString();
        }
    }

    public void OpenOptions()
    {
        if (GameManager.optionsMenu)
        {
            GameManager.optionsMenu.OpenMenu();
            GameManager.optionsMenu.onClose += OptionsMenu_onClose;
        }

        StartCoroutine(moveMain(-1, 10));
    }

    private void OptionsMenu_onClose()
    {
        GameManager.optionsMenu.onClose -= OptionsMenu_onClose;
        StartCoroutine(moveMain(1, 4));
    }

    /// <summary>
    /// Moves main menu
    /// </summary>
    /// <param name="dir">move direction right = 1 left = -1</param>
    /// <param name="duration">the distance it will cover each second</param>
    /// <returns></returns>
    IEnumerator moveMain(int dir, float duration)
    {
        stopMenuMove = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        stopMenuMove = false;

        float l = MainMenu.rect.width * MainMenu.localScale.x;

        float endPoint = 0;
        if (dir > 0)
            endPoint = 0;
        else if (dir < 0)
            endPoint = -l;

        Vector2 start, end;
        start = MainMenu.anchoredPosition;
        end = new Vector2(endPoint, start.y);

        float t = 0;
        
        if (dir > 0)
        {
            while (MainMenu.anchoredPosition.x - endPoint < -0.05)
            {
                t += Time.deltaTime / duration;
                start = MainMenu.anchoredPosition;
                MainMenu.anchoredPosition = Vector2.Lerp(start, end, t);

                if (stopMenuMove)
                    yield break;

                yield return new WaitForEndOfFrame();
            }
        }
        else if (dir < 0)
        {
            while (MainMenu.anchoredPosition.x - endPoint > 0.05)
            {
                t += Time.deltaTime / duration;
                start = MainMenu.anchoredPosition;
                MainMenu.anchoredPosition = Vector2.Lerp(start, end, t);

                if (stopMenuMove)
                    yield break;

                yield return new WaitForEndOfFrame();
            }
        }
    }
    bool stopMenuMove;
}
