using UnityEngine;
using System.Collections;

public class UIControler : MonoBehaviour
{
    public AttackSlider attackSlider;


    [SerializeField]
    MaskBar heatLevel,EnergyLevel;
    [SerializeField]
    UnityEngine.UI.Slider TimerBar;
    bool GamePaused;
    bool PlayerExists;

    bool StageControlerAlive = false;

    public void Start()
    {
        GameManager.instance.onPauseGame += onGamePause;
        PlayerControler.onPlayerCreated += PlayerControler_onPlayerCreated;
        PlayerControler.onPlayerDestoryed += PlayerControler_onPlayerDestoryed;

        StageControler.onBossBattleBegins += StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds += StageControler_onBossBattleEnds;


        attackSlider.gameObject.SetActive(false);
    }

    public void Destory()
    {
        GameManager.instance.onPauseGame -= onGamePause;
        PlayerControler.onPlayerCreated -= PlayerControler_onPlayerCreated;
        PlayerControler.onPlayerDestoryed -= PlayerControler_onPlayerDestoryed;


        StageControler.onBossBattleBegins -= StageControler_onBossBattleBegins;
        StageControler.onBossBattleEnds -= StageControler_onBossBattleEnds;
    }

    private void StageControler_onBossBattleEnds()
    {
        attackSlider.gameObject.SetActive(false);
    }

    private void StageControler_onBossBattleBegins()
    {
        attackSlider.gameObject.SetActive(true);
    }

    private void PlayerControler_onPlayerDestoryed(PlayerControler p)
    {
        PlayerExists = false;
    }

    private void PlayerControler_onPlayerCreated(PlayerControler p)
    {
        PlayerExists = true;
    }

    void onGamePause(bool b)
    {
        GamePaused = b;
    }

    #region Update
    void Update()
    {
        if (GamePaused)
            return;

        Update_HeatLevel();
        Update_EnergyLevel();
        Update_TimerBar();
        Update_AttackBarVisable();
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

    void Update_TimerBar()
    {
        if (!PlayerExists || !GameManager.stageControler)
            return;

        TimerBar.value = GameManager.stageControler.NormalizedTimeLeft();
    }

    void Update_AttackBarVisable()
    {
        if (GameManager.stageControler)
            if (GameManager.stageControler.bossDefeated)
                if (attackSlider.isActiveAndEnabled)
                    attackSlider.gameObject.SetActive(false);
    }
    #endregion
}
