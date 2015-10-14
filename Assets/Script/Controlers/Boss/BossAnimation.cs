using UnityEngine;
using System.Collections;

public class BossAnimation : MonoBehaviour
{
    
    void Start()
    {
        
    }

    public virtual void Fight_onPerfectHit()
    {
        Debug.Log("NOT BAD! A PERFECT HIT");
    }

    public virtual void Fight_onGoodHit()
    {
        Debug.Log("PLAYER DID A NOT SO BAD HIT");
    }

    public virtual void Fight_onBadHit()
    {
        Debug.Log("PLAYER CAN'T AIM!");
    }

    public virtual void OnPlayerHit()
    {
        Debug.Log("PLAYER HIT ME");
    }

    public virtual void OnEnemyHit()
    {
        Debug.Log("Enemie HIT ANIMATION PLAY Event");
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
