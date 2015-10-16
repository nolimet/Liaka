using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D))]
public class PickupBase :BaseObject {
    public delegate void PickupObjectEvent(PickupType p);
    public static event PickupObjectEvent onPickup;

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
    public override void startBehaviours()
    {
        base.startBehaviours();
        type = objectType.Pickup;
        stopedMoving = false;
    }

    public override void RemoveFromView()
    {
        unregisterDelegates();
        PickupPool.RemoveObject(this);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.transform.tag)
        {
            case TagManager.Ground:
                stopedMoving = true;
                break;
        }
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

    void FixedUpdate()
    {
        if (!GameManager.playerControler || !stopedMoving)
            return;

        Vector2 v;
        Vector2 d = GameManager.playerControler.transform.position - transform.position;
        v = util.MathHelper.AngleToVector(util.MathHelper.VectorToAngle(d));

        if (!rigi)
            rigi = GetComponent<Rigidbody2D>();

        if (!rigi)
            Debug.LogError(name + " : MISSES RIGIDBODY!");

        rigi.AddForce(v * 50f);
    }
}
