using UnityEngine;
using System.Collections;

public class GroundEnemy : EnemyBase
{

    bool running;
    bool g;
    string groundTag = "";

    float jumpCooldown = 0f;

    Vector3 rayCastOffSet = Vector3.zero;
    int RayMask;
    protected override void Awake()
    {
        base.Awake();
        fadeWhenGroundhit = false;
        ri.constraints = RigidbodyConstraints2D.FreezeRotation;

        RayMask = 0;
        RayMask = 1 << LayerMask.NameToLayer(TagManager.Ground);

        Bounds b = cp.bounds;
        rayCastOffSet = b.extents;
        rayCastOffSet.y *= -1;
        rayCastOffSet.x *= 0.5f;
    }

    #region overrides
    public override void startBehaviours()
    {
        base.startBehaviours();
        ri.velocity = Vector3.zero;
        ri.isKinematic = false;
        alive = true;
        StartCoroutine(MoveBehaviourLoop());
        setVelocity(new Vector2(-5, 0));
    }

    protected override IEnumerator MoveBehaviourLoop()
    {
        if (running)
            yield break;


        running = true;
        float d;
        System.DateTime t1;
        while (alive)
        {
            if (!GameManager.playerControler)
                break;

            if (!GameManager.gamePaused)
            {
                d = transform.position.x - GameManager.playerControler.transform.position.x;
                if (d < 0)
                    d *= -1;

                if (d < 1f && jumpCooldown <= 0)
                {
                    Jump();
                }
            }

            t1 = System.DateTime.Now;

            yield return new WaitForSeconds(0.1f);
            if (!paused)
                if (jumpCooldown > 0)
                    jumpCooldown -= (float)(System.DateTime.Now - t1).TotalMilliseconds / 1000;
        }

        running = false;
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
            jumpCooldown = 2.5f;
            ri.AddForce(new Vector3(0, 9 * ri.mass, 0), ForceMode2D.Impulse);
        }

        g = false;
    }

    RaycastHit2D hitcast;
    void FixedUpdate()
    {
        if (paused && !alive)
            return;

       // Debug.DrawLine(transform.position + rayCastOffSet, transform.position + rayCastOffSet + (Vector3.down * 1f), Color.green, 1f);
        hitcast = Physics2D.Raycast(transform.position + rayCastOffSet, Vector2.down, 0.75f, RayMask);       

        if (!hitcast.transform)
        {
            Debug.DrawLine(transform.position + rayCastOffSet, hitcast.point,Color.white);
            if (g)
                Jump();
            g = false;
        }
        else if (hitcast.transform.tag == TagManager.Ground)
        {
            Debug.DrawLine(transform.position + rayCastOffSet, hitcast.point,Color.red);
            g = true;
        }

        hitcast = new RaycastHit2D();
    }

}
