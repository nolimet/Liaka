using UnityEngine;
using System.Collections;

public class UIControler : MonoBehaviour
{

    [SerializeField]
    MaskBar heatLevel,EnergyLevel;
    bool GamePaused;
    bool PlayerExists;
    public void Start()
    {
        GameManager.instance.onPauseGame += onGamePause;
        PlayerControler.onPlayerCreated += PlayerControler_onPlayerCreated;
        PlayerControler.onPlayerDestoryed += PlayerControler_onPlayerDestoryed;

        Debug.Log("start");
    }

    private void PlayerControler_onPlayerDestoryed(PlayerControler p)
    {
        PlayerExists = false;
    }

    private void PlayerControler_onPlayerCreated(PlayerControler p)
    {
        PlayerExists = true;
    }

    public void Destory()
    {
        GameManager.instance.onPauseGame -= onGamePause;
        PlayerControler.onPlayerCreated -= PlayerControler_onPlayerCreated;
        PlayerControler.onPlayerDestoryed -= PlayerControler_onPlayerDestoryed;
        
    }

    void onGamePause(bool b)
    {
        GamePaused = b;
    }


    void Update()
    {
        if (GamePaused)
            return;

        Update_HeatLevel();
        Update_EnergyLevel();
    }

    void Update_HeatLevel()
    {
        if (!PlayerExists)
            return;
       
        heatLevel.setValue(GameManager.playerControler.weaponHeat / GameManager.playerControler.MaxHeat);

    }

    void Update_EnergyLevel()
    {
        if (!PlayerExists)
            return;

        EnergyLevel.setValue(GameManager.playerControler.Energy / GameManager.playerControler.MaxEnergy);
    }
}
