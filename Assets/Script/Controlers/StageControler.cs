using UnityEngine;
using System.Collections;

public class StageControler : MonoBehaviour
{
    public delegate void StageControlerEvent(StageControler stage);
    public static event StageControlerEvent onStageCreated, onStageDestroyed;
    public delegate void DelegateVoid();
    public static event DelegateVoid onStageTimerEnded, onBossBattleBegins, onBossBattleEnds;
    [Tooltip("The time in second the stage takes")]
    public float StageLength;
    float TimeLeft;

    // Use this for initialization
    void Start()
    {
        TimeLeft = StageLength;
        if (onStageCreated != null)
            onStageCreated(this);

        if (onBossBattleEnds != null)
            onBossBattleEnds();
    }

    public void OnDestroy()
    {
        if (onStageDestroyed != null)
            onStageDestroyed(this);
    }

    // Update is called once per frame
    void Update()
    {
        TimeLeft -= Time.deltaTime;
        if(TimeLeft<0 && TimeLeft>-1)
        {
            TimeLeft = -20;
            if (onStageTimerEnded != null)
                onStageTimerEnded();
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
        return TimeLeft / StageLength;
    }
}
