using UnityEngine;
using System.Collections;

public class StageControler : MonoBehaviour
{
    public delegate void StageControlerEvent(StageControler stage);
    public static event StageControlerEvent onStageCreated, onStageDestroyed;
    public delegate void DelegateVoid();
    public static event DelegateVoid onStageEnded;
    [Tooltip("time in seconds")]
    public float StageLength;
    float TimeLeft;

    // Use this for initialization
    void Start()
    {
        TimeLeft = StageLength;
        if (onStageCreated != null)
            onStageCreated(this);
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
            if (onStageEnded != null)
                onStageEnded();
        }
    }

    public float NormalizedTimeLeft()
    {
        return TimeLeft / StageLength;
    }
}
