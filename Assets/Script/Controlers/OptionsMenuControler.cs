using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuControler : MonoBehaviour {

    public delegate void voidDelegate();
    public event voidDelegate onClose;

    [SerializeField]
    private Slider SFX_Slider, Music_Slider, Interface_Slider;

    void Start()
    {
        SaveData d = GameManager.instance.saveDat;

        SFX_Slider.value = d.options.soundVolume;
        Music_Slider.value = d.options.musicVolume;
        Interface_Slider.value = d.options.interfaceVolume;

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

        gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
    }
}
