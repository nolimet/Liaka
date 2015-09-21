using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuControler : MonoBehaviour {

    [System.Serializable]
    public struct opMenu
    {
        public Slider sounds, music;
        public GameObject menu;
    }
    public opMenu OptionMenu;

    void Start()
    {
        OpenOptions(false);
        OptionMenu.sounds.value = GameManager.instance.saveDat.soundVolume;
        OptionMenu.music.value = GameManager.instance.saveDat.musicVolume;
    }

    public void OpenOptions(bool b)
    {
        OptionMenu.menu.SetActive(b);
    }


    public void onFXVolumeChange(float f)
    {
        GameManager.instance.saveDat.soundVolume = f;
    }

    public void onMusicVolumeChange(float f)
    {
        GameManager.instance.saveDat.musicVolume = f;
    }
}
