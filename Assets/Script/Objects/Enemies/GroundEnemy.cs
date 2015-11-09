using UnityEngine;
using System.Collections;

public class GroundEnemy : EnemyBase
{

    bool running;
    bool g;
    string groundTag = "";

    float jumpCooldown  = 0f;

    Vector3 rayCastOffSet = Vector3.zero;
    int RayMask;
    void Start()
    {
        fadeWhenGroundhit = false;
        ri.constraints = RigidbodyConstraints2D.FreezeRotation;

        RayMask = 1 << LayerMask.NameToLayer("Ground");
        Bounds b = cp.bounds;
        rayCastOffSet = b.extents;
        rayCastOffSet.y *= -1;
    }

    #region overrides
    public override void startBehaviours()
    {
        base.startBehaviours();
        ri.velocity = Vector3.zero;
        ri.isKinematic = false;
        StartCoroutine(MoveBehaviourLoop());
        setVelocity(new Vector2(-5, 0));
    }

    protected override IEnumerator MoveBehaviourLoop()
    {
        if (running)
            yield break;

        float d;
        System.DateTime t1;
        while (alive)
        {
            if (!GameManager.playerControler)
                yield break;

            if (!paused)
            {
                d = transform.position.x - GameManager.playerControler.transform.position.x;
                if (d < 0)
                    d *= -1;

                if (d < 1f && jumpCooldown <= 0)
                {
                    Jump();
                    jumpCooldown = 1f;
                }
            }

            t1 = System.DateTime.Now;

            yield return new WaitForSeconds(0.1f);
            if (!paused)
                if (jumpCooldown > 0)
                    jumpCooldown -= (float)(System.DateTime.Now - t1).TotalMilliseconds / 1000;
        }
    }

    protected override IEnumerator fadeOut(float f, float d, string g = "", bool DisableCollsionAtStart = true)
    {
        ri.isKinematic = true;
        return base.fadeOut(f, d, g, DisableCollsionAtStart);
    }
    #endregion

    void Jump()
    {
        if (g && jumpCooldown <= 0)
        {
            jumpCooldown = 1f;
            ri.AddForce(new Vector3(0, 9 * ri.mass, 0), ForceMode2D.Impulse);
        }

        g = false;
    }

    RaycastHit2D hitcast;
    void FixedUpdate()
    {
        if (paused)
            return;

        hitcast = Physics2D.Raycast(transform.position + rayCastOffSet, Vector2.down, 0.01f, RayMask);

        if (!hitcast.transform)
        {
            if (g)
                Jump();
            g = false;
        }
        else
        {
            g = true;
        }

        hitcast = new RaycastHit2D();
    }

}
