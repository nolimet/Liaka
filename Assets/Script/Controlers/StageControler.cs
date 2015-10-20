using UnityEngine;
using System.Collections;

public class StageControler : MonoBehaviour
{
    public delegate void StageControlerEvent(StageControler stage);
    public static event StageControlerEvent onStageCreated, onStageDestroyed;
    public delegate void DelegateVoid();
    public delegate void DelegateBool(bool b);
    public static event DelegateVoid onStageTimerEnded, onBossBattleBegins, onBossBattleEnds;
    [Tooltip("The time in seconds the stage takes")]
    public float StageLength, BossBattleLength;
    float TimeLeft;
    bool bossFighting,bossDefeated;
    BossControler bossControler;

    // Use this for initialization
    void Start()
    {
        TimeLeft = StageLength;
        if (onStageCreated != null)
            onStageCreated(this);

        if (onBossBattleEnds != null)
            onBossBattleEnds();

        PlayerControler.onPlayerCreated += PlayerControler_onPlayerCreated;
        BossControler.onDefeated += BossControler_Defeated;
    }

    public void OnDestroy()
    {
        if (onStageDestroyed != null)
            onStageDestroyed(this);

        PlayerControler.onPlayerCreated -= PlayerControler_onPlayerCreated;
        BossControler.onDefeated -= BossControler_Defeated;
    }

    private void PlayerControler_onPlayerCreated(PlayerControler p)
    {
        p.onDeath += Player_onDeath;
        p.onEnergyZero += Player_onEnergyZero;
    }

    private void Player_onEnergyZero()
    {
       
    }

    private void Player_onDeath()
    {
        Application.LoadLevel("GAME-OVER");
    }

    private void BossControler_Defeated()
    {
        bossDefeated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeLeft>0)
            TimeLeft -= Time.deltaTime;

        if (TimeLeft<0 && TimeLeft>-1)
        {
            TimeLeft = -20;
            if (onStageTimerEnded != null)
                onStageTimerEnded();

            if (bossFighting)
            {
                if (onBossBattleEnds != null)
                    onBossBattleEnds();

                TimeLeft = StageLength;
                bossFighting = false;
            }
            else
            {
                if (onBossBattleBegins != null)
                    onBossBattleBegins();

                bossFighting = true;
                TimeLeft = BossBattleLength;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            if (onBossBattleBegins != null)
                onBossBattleBegins();

        if (Input.GetKeyDown(KeyCode.H))
            if (onBossBattleEnds != null)
                onBossBattleEnds();
    }

    public float NormalizedTimeLeft()
    {
        if (!bossFighting)
            return TimeLeft / StageLength;
        else
            return TimeLeft / BossBattleLength;
    }
}
