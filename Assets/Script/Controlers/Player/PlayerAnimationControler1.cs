using UnityEngine;
using System.Collections;

public class PlayerAnimationControler1 : MonoBehaviour
{

    SpriteRenderer[] r;

    public Animator ani;
    Rigidbody2D ri;

    void Awake()
    {
        r = GetComponentsInChildren<SpriteRenderer>();
        ri = GetComponent<Rigidbody2D>();
        PlayerControler.onPlayerCreated += PlayerControler_onPlayerCreated;
        PlayerControler.onPlayerDestoryed += PlayerControler_onPlayerDestoryed;
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
    }

    private void PlayerControler_onPlayerCreated(PlayerControler p)
    {
        p.onJump += Player_OnJump;
        p.onCoinsLost += Player_OnHit;
        p.onHitGround += Player_Land;
        p.onShoot += P_onShoot;
    }

    private void P_onShoot()
    {
        ani.SetTrigger("Shooting");
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
        ani.SetTrigger("Jump");
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
    }


    public void Player_OnPause(bool b)
    {

    }

    float f;

    void Update()
    {
        f = ri.velocity.y;
        if (f < 0)
            f = -f;
        ani.SetFloat("FallSpeed",f);
    }
}
