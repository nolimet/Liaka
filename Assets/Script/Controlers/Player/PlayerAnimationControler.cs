using UnityEngine;
using System.Collections;

public class PlayerAnimationControler : MonoBehaviour
{

    //int CurrentMoveDir = 0;

    [SerializeField]
    SkeletonAnimation anim;
    float shootAniHold = 0f;
    float startingTimeScale = 0f;
    bool eventPlaced = false;
    #region Animations
    [SpineAnimation]
    public string hit;

    [SpineAnimation]
    public string idle;

    //[SpineAnimation]
    //public string MoveLeft;

    //[SpineAnimation]
    //public string MoveRight;

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

    [SpineAnimation]
    public string overHeated;

    [SpineAnimation]
    public string coolDown;
    #endregion

    void Start()
    {
        if (!anim)
        {
            Destroy(this);
            return;
        }

        if (!GameManager.playerControler)
            return;

        startingTimeScale = anim.timeScale;

        eventPlaced = true;

        GameManager.playerControler.onJump += Player_OnJump;
        GameManager.playerControler.onCoinsLost += Player_OnHit;
        GameManager.playerControler.onHitGround += Player_Land;
        GameManager.playerControler.onShoot += Player_Shoot;
        GameManager.instance.onPauseGame += Instance_onPauseGame;
    }

    private void Instance_onPauseGame(bool b)
    {
        if (b)
            anim.timeScale = 0f;
        else
            anim.timeScale = startingTimeScale;
    }

    public void OnDestroy()
    {
        if (!GameManager.playerControler || !eventPlaced)
            return;

        eventPlaced = false;

        GameManager.playerControler.onJump -= Player_OnJump;
        GameManager.playerControler.onCoinsLost -= Player_OnHit;
        GameManager.playerControler.onHitGround -= Player_Land;
        GameManager.playerControler.onShoot -= Player_Shoot;
        GameManager.instance.onPauseGame -= Instance_onPauseGame;
    }

    public void OnDisable()
    {
        if (!eventPlaced || !GameManager.playerControler)
            return;

        eventPlaced = false;

        GameManager.playerControler.onJump -= Player_OnJump;
        GameManager.playerControler.onCoinsLost -= Player_OnHit;
        GameManager.playerControler.onHitGround -= Player_Land;
        GameManager.playerControler.onShoot -= Player_Shoot;
    }

    void Update()
    {
        Update_shootAniHold();
        if (!eventPlaced)
            Start();
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

    public void Player_Death()
    {
        anim.state.SetAnimation(0, death, false);
    }

    public void Player_Shoot()
    {
        if (shootAniHold <= 0)
            anim.state.SetAnimation(1, shoot, false);
        shootAniHold = 1f;
    }

    public void Update_shootAniHold()
    {
        if (shootAniHold <= 0 && shootAniHold > -1)
        {
            anim.state.SetAnimation(1, idle, false);
            shootAniHold = -20;
        }
        else if (shootAniHold > 0)
        {
            shootAniHold -= Time.deltaTime;
        }
    }

    public void Player_MoveChange(int Dir)
    {
        switch (Dir)
        {
            case -1:
                anim.AnimationName = idle;
                break;

            case 0:
                anim.AnimationName = idle;
                break;

            case 1:
                anim.AnimationName = idle;
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
