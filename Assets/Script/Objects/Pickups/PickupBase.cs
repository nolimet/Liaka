using UnityEngine;
using System.Collections;
using util;

[RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D))]
public class PickupBase :BaseObject {
    public delegate void PickupObjectEvent(PickupType p);
    public static event PickupObjectEvent onPickup,onGroundHit;

    public enum PickupType
    {
        Coin,
        Energy,
        SpeedUp,
        SpeedDown
    }

    public enum Movement
    {
        Static,
        Dynamic
    }

    public Movement moveType
    {
        get { return _moveType; }
        set { _moveType = value; SetMovementType(value); }
    }

    public PickupType pType;
    public Movement _moveType;
    Rigidbody2D rigi;
    bool stopedMoving;
    bool g;
    int RayCastMask;

    protected override void Awake()
    {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer("Pickup");
        type = objectType.Pickup;
        RayCastMask = 0;
        RayCastMask = 1 << LayerMask.NameToLayer(TagManager.Ground);
    }

    public override void startBehaviours()
    {
        base.startBehaviours();
        if(gameObject.layer != LayerMask.NameToLayer("Pickup"))
            gameObject.layer = LayerMask.NameToLayer("Pickup");
        stopedMoving = false;
    }

    public override void RemoveFromView(bool callRemoveOjbect = true)
    {
        unregisterDelegates();
        alive = false;

        ri.velocity = Vector2.zero;
        ri.isKinematic = true;

        if (a)
            a.enabled = false;

        if (SA)
            SA.timeScale = 0;

        if (callRemoveOjbect)
            PickupPool.RemoveObject(this);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.transform.tag)
        {
            case TagManager.Ground:
                stopedMoving = true;

                
                break;

            case TagManager.Trap:
                stopedMoving = true;
                FadeOutStart(0.3f, 0);
                break;
        }
    }

    public void ignorPlayer()
    {
        gameObject.layer = LayerMask.NameToLayer("IgnorPlayer");
    }

    public virtual void SetMovementType(Movement t)
    {
        // moveType = t;
        if (!rigi)
            rigi = GetComponent<Rigidbody2D>();

        rigi.constraints = RigidbodyConstraints2D.None;
        if (t == Movement.Dynamic)
            rigi.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rigi.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    
    public virtual void Unstuck()
    {
        int mask = 1 << LayerMask.NameToLayer("Ground");

        Vector3 pos = transform.position;
        Vector3 distOff = Vector3.zero;
        float l = GetComponent<CircleCollider2D>().bounds.size.y/2f;


        RaycastHit2D hit = Physics2D.Raycast(pos + distOff, new Vector2(0, -1), l, mask);
        Debug.DrawLine(transform.position + distOff, transform.position + distOff + new Vector3(0, -l), Color.red,20f);

       // Debug.Log(l);
        if (hit && hit.transform.tag == TagManager.Ground)
        {
            Transform oStuckin = hit.transform;
            while (hit.transform == oStuckin)
            {
                hit = Physics2D.Raycast(pos + distOff, new Vector2(0, -1), l, mask);
                distOff += new Vector3(0, 0.05f);
               // Debug.Log(hit.transform);
            }

            transform.position = pos + distOff;
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag != TagManager.Enemy && collision.tag != TagManager.Pickup)
            if (collision.name.ToUpper().Contains("BOX") && useBounds.touchingBound(collision.name))
            {
                StartCoroutine(fadeOut(0.1f, 0.6f));
            }

        switch (collision.transform.tag)
        {
            case TagManager.Player:
                StartCoroutine(fadeOut(0.1f, 0));

                collision.transform.gameObject.SendMessage("hitPickup", gameObject, SendMessageOptions.DontRequireReceiver);
                if (onPickup != null)
                    onPickup(pType);
                break;

           
        }
    }

    RaycastHit2D hitcast;
    protected override void Update()
    {
        base.Update();

        if (alive)
        {
            hitcast = new RaycastHit2D();

            if (SA)
            {
                hitcast = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, SA.GetComponent<Renderer>().bounds.size.y / 2f), Vector2.down, 0.01f, RayCastMask);
            }
            else if (a)
            {
                Physics2D.Raycast((Vector2)transform.position + new Vector2(0, transform.getChildBounds().size.y / 2f), Vector2.down, 0.01f, RayCastMask);
            }

            if (hitcast.transform)
            {
                if (!g)
                {
                    if (onGroundHit != null)
                        onGroundHit(pType);
                    g = true;
                }
            }
            else
            {
                g = false;
            }
        }
    }

    /// <summary>
    /// fades the object out
    /// </summary>
    /// <param name="f">Time the fading takes</param>
    /// <param name="d">Starting Delay before the fading</param>
    public void FadeOutStart(float f, float d)
    {
        StartCoroutine(fadeOut(f, d, "", false));
    }

    void FixedUpdate()
    {
        if (!GameManager.playerControler || !stopedMoving)
            return;

        Vector2 v;
        Vector2 d = GameManager.playerControler.transform.position - transform.position;
        v = util.Common.AngleToVector(util.Common.VectorToAngle(d));

        if (!rigi)
            rigi = GetComponent<Rigidbody2D>();

        if (!rigi)
            Debug.LogError(name + " : MISSES RIGIDBODY!");

        rigi.AddForce(v * 50f);
    }
}
