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

    int updateDelay;

    void Start()
    {
        OpenOptions(false);
        OptionMenu.sounds.value = GameManager.instance.saveDat.options.soundVolume;
        OptionMenu.music.value = GameManager.instance.saveDat.options.musicVolume;
        OptionMenu.interfaceVol.value = GameManager.instance.saveDat.options.interfaceVolume;

        CoinsDisplay.text = "Current Gold:" + "\n " + GameManager.instance.saveDat.game.CoinsCurrent.ToString();
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
}
