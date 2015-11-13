using UnityEngine;
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
        OpenOptions(false);
        OptionMenu.sounds.value = GameManager.instance.saveDat.options.soundVolume;
        OptionMenu.music.value = GameManager.instance.saveDat.options.musicVolume;
        OptionMenu.interfaceVol.value = GameManager.instance.saveDat.options.interfaceVolume;

        CoinsDisplay.text = "Current Gold:" + "\n " + GameManager.instance.saveDat.game.CoinsCurrent.ToString();

        MainMenu.anchoredPosition = MainMenu.anchoredPosition - new Vector2(MainMenu.rect.width, 0);
        StartCoroutine(moveMain(1, 20));
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

    public void OpenOptions(bool b)
    {
        OptionMenu.menu.SetActive(b);
        if(b)
            StartCoroutine(moveMain(-1, 10));
        else
            StartCoroutine(moveMain(1, 20));
    }

    public void onFXVolumeChange(float f)
    {
        GameManager.instance.saveDat.options.soundVolume = f;
    }

    public void onMusicVolumeChange(float f)
    {
        GameManager.instance.saveDat.options.musicVolume = f;
    }

    public void onInterfaceVolumeChange(float f)
    {
        GameManager.instance.saveDat.options.interfaceVolume = f;
    }

    public void ClearSaveData()
    {
        GameManager.instance.ResetSave();
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
