using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuControler : MonoBehaviour {

    public delegate void voidDelegate();
    public event voidDelegate onClose;

    [SerializeField]
    RectTransform RectTrans = null;

    [SerializeField]
    private Slider SFX_Slider = null, Music_Slider = null, Interface_Slider = null;

    [SerializeField]
    private Toggle godModeToggle = null, DebugToggle = null;

    void Start()
    {
        SaveData d = GameManager.instance.saveDat;

        SFX_Slider.value = d.options.soundVolume;
        Music_Slider.value = d.options.musicVolume;
        Interface_Slider.value = d.options.interfaceVolume;

        if (godModeToggle)
            godModeToggle.isOn = GameManager.instance.GodMode;

        if (DebugToggle)
            DebugToggle.isOn = GameManager.AllowDebug;

        GameManager.instance.onPauseGame += Instance_onPauseGame;
        gameObject.SetActive(false);
    }

    private void Instance_onPauseGame(bool b)
    {
        if (!b)
            CloseMenu();
    }

    public void update_SFXVolume(float f)
    {
        GameManager.instance.saveDat.options.soundVolume = f;
    }

    public void update_MusicVolume(float f)
    {
        GameManager.instance.saveDat.options.musicVolume = f;
    }

    public void update_InterfaceVolume(float f)
    {
        GameManager.instance.saveDat.options.interfaceVolume = f;
    }

    public void ClearSaveData()
    {
        GameManager.instance.ResetSave();
    }

    public void CloseMenu()
    {
        if (onClose != null)
            onClose();

        if (gameObject.activeSelf)
            StartCoroutine(moveMain(-1, 4, changeStateToDisabledAtEnd: true));
    }

    public void ToggleGodMode(bool b)
    {
        GameManager.instance.GodMode = b;
    }

    public void ToggleDebug(bool b)
    {
        GameManager.instance._AllowDebug = b;
    }

    public void OpenMenu()
    {
       gameObject.SetActive(true);

        StartCoroutine(moveMain(1, 10,true));
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
            endPoint = -l;

        if (startAtEnd && dir > 0)
            RectTrans.anchoredPosition = new Vector2(RectTrans.anchoredPosition.x, -l);
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
            while (RectTrans.anchoredPosition.y + endPoint < -10)
            {
              //  Debug.Log(" POS : " + (RectTrans.anchoredPosition.y - endPoint) + " DIR : " + dir);

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

               // Debug.Log(" POS : " + (RectTrans.anchoredPosition.y - endPoint) + " DIR : " + dir);

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

       // Debug.Log("end option menu move");
    }
    bool stopMenuMove;
}
