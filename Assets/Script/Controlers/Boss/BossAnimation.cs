using UnityEngine;
using System.Collections;

public class BossAnimation : MonoBehaviour
{
    public SkeletonAnimation ani;
    float startingTimeScale = 0f;

    [SpineAnimation]
    public string walkNormal;
    void Start()
    {
        GameManager.instance.onPauseGame += Instance_onPauseGame;

        if (!ani)
            ani = GetComponent<SkeletonAnimation>();

        if (!ani)
            ani = GetComponentInChildren<SkeletonAnimation>();

        if (!ani)
        {
            enabled = false;
            return;
        }

        startingTimeScale = ani.timeScale;

        
    }

    public void OnDestroy()
    {
        GameManager.instance.onPauseGame -= Instance_onPauseGame;
    }

    private void Instance_onPauseGame(bool b)
    {
        if (b)
            ani.timeScale = 0f;
        else
            ani.timeScale = startingTimeScale;
    }

    public virtual void Fight_onPerfectHit()
    {
        // Debug.Log("NOT BAD! A PERFECT HIT");
    }

    public virtual void Fight_onGoodHit()
    {
        // Debug.Log("PLAYER DID A NOT SO BAD HIT");
    }

    public virtual void Fight_onBadHit()
    {
        //Debug.Log("PLAYER CAN'T AIM!");
    }

    public virtual void OnPlayerHit()
    {
        //Debug.Log("PLAYER HIT ME");
    }

    public virtual void OnEnemyHit()
    {
        //Debug.Log("Enemie HIT ANIMATION PLAY Event");
    }

    public virtual void MoveChange(int interger)
    {
        switch (interger)
        {
            case -1:
                //right
                break;

            case 0:
                //idle
                break;

            case 1:
                //left
                break;
        }
    }
}
