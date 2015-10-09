using UnityEngine;
using System.Collections;

public class PlayerAnimationControler : MonoBehaviour {

    int CurrentMoveDir = 0;
    SpineAnimation anim;
	public void Player_OnHit()
    {

    }

    public void Player_OnJump()
    {

    }

    public void Player_Land()
    {

    }

    public void Player_MoveChange(int Dir)
    {
        switch (Dir)
        {
            case -1:
                //left
                break;

            case 0:
                //idle
                break;

            case 1:
                //right
                break;
        }

        CurrentMoveDir = Dir;
    }


    public void Player_OnPause(bool b)
    {
        //pause all animations
    }
}
