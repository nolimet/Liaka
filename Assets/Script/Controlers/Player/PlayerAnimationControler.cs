using UnityEngine;
using System.Collections;

public class PlayerAnimationControler : MonoBehaviour
{

    //int CurrentMoveDir = 0;

    [SerializeField]
    SkeletonAnimation anim;

    [SpineAnimation]
    public string hit;

    [SpineAnimation]
    public string idle;

    [SpineAnimation]
    public string MoveLeft;

    [SpineAnimation]
    public string MoveRight;

    [SpineAnimation]
    public string jump;

    [SpineAnimation]
    public string Idle_Air;

    [SpineAnimation]
    public string shoot;

    [SpineAnimation]
    public string death;

    [SpineAnimation]
    public string groundHit;

    void Start()
    {
        anim = GetComponent<SkeletonAnimation>();

        if (!anim)
        {
            Destroy(this);
            return;
        }
        GameManager.playerControler.onJump += Player_OnJump;
        GameManager.playerControler.onCoinsLost += Player_OnHit;
        GameManager.playerControler.onHitGround += Player_Land;
    }

    public void OnDestroy()
    {
        if (!GameManager.playerControler)
            return;

        GameManager.playerControler.onJump -= Player_OnJump;
        GameManager.playerControler.onCoinsLost -= Player_OnHit;
        GameManager.playerControler.onHitGround -= Player_Land;
    }

    public void Player_OnHit(int i)
    {
        anim.state.SetAnimation(0, hit, false);
        anim.state.AddAnimation(0, idle, true, 0);
    }

    public void Player_OnJump()
    {
        anim.state.SetAnimation(0, jump, false);
        anim.state.AddAnimation(0, Idle_Air, true, 0);
    }

    public void Player_Land()
    {
        anim.state.SetAnimation(0, groundHit, false);
        anim.state.AddAnimation(0, idle, true, 0);
    }

    public void Player_MoveChange(int Dir)
    {
        switch (Dir)
        {
            case -1:
                anim.AnimationName = MoveLeft;
                break;

            case 0:
                anim.AnimationName = idle;
                break;

            case 1:
                anim.AnimationName = MoveRight;
                break;
        }

        //CurrentMoveDir = Dir;
    }


    public void Player_OnPause(bool b)
    {
        if (b)
            anim.timeScale = 0;
        else
            anim.timeScale = 1;
    }
}
