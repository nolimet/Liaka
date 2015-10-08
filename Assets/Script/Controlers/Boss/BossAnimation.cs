using UnityEngine;
using System.Collections;

public class BossAnimation : MonoBehaviour
{
    
    void Start()
    {
        
    }

    public void Fight_onPerfectHit()
    {
        Debug.Log("NOT BAD! A PERFECT HIT");
    }

    public void Fight_onGoodHit()
    {
        Debug.Log("PLAYER DID A NOT SO BAD HIT");
    }

    public void Fight_onBadHit()
    {
        Debug.Log("PLAYER CAN'T AIM!");
    }

    public void OnPlayerHit()
    {
        Debug.Log("PLAYER HIT ME");
    }

    public void OnEnemyHit()
    {
        Debug.Log("Enemie HIT ANIMATION PLAY Event");
    }

    public void MoveChange(int interger)
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
