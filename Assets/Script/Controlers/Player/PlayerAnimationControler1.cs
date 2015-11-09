using UnityEngine;
using System.Collections;

public class PlayerAnimationControler1 : MonoBehaviour
{

    int CurrentMoveDir = 0;
    SpriteRenderer[] r;

    void Awake()
    {
        r = GetComponentsInChildren<SpriteRenderer>();

        PlayerControler.onPlayerCreated += PlayerControler_onPlayerCreated;
        PlayerControler.onPlayerDestoryed += PlayerControler_onPlayerDestoryed;
        Debug.Log("bleep");
    }

    public void OnDestroy()
    {
        PlayerControler.onPlayerCreated -= PlayerControler_onPlayerCreated;
        PlayerControler.onPlayerDestoryed -= PlayerControler_onPlayerDestoryed;
    }

    private void PlayerControler_onPlayerDestoryed(PlayerControler p)
    {
        p.onJump -= Player_OnJump;
        p.onCoinsLost -= Player_OnHit;
        p.onHitGround -= Player_Land;
        Debug.Log("pling");
        
    }

    private void PlayerControler_onPlayerCreated(PlayerControler p)
    {
        p.onJump += Player_OnJump;
        p.onCoinsLost += Player_OnHit;
        p.onHitGround += Player_Land;
    }

    public void Player_OnHit(int i)
    {
        StartCoroutine(blink());
    }

    bool blinking;
    IEnumerator blink()
    {
        if (blinking)
            yield break;

        blinking = true;
        float t = 0.3f;

        bool s = false;
        Color A, B;
        A = Color.red;
        B = Color.white;

        while(t>0)
        {
            t -= Time.deltaTime;

            if (s)
                ChangeColour(A);
            else
                ChangeColour(B);

            s = !s;
            yield return new WaitForEndOfFrame();
        }

        blinking = false;
        ChangeColour(Color.white);
    }

    void ChangeColour(Color c)
    {
        foreach (SpriteRenderer s in r)
            s.color = c;
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
                
                break;

            case 0:
                
                break;

            case 1:
                
                break;
        }

        CurrentMoveDir = Dir;
    }


    public void Player_OnPause(bool b)
    {

    }
}
