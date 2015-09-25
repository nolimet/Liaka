using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D))]
public class PickupBase :BaseObject {
    public delegate void PickupObjectEvent(PickupType p);
    public static event PickupObjectEvent onPickup;

    public enum PickupType
    {
        Coin,
        Energy
    }

    public enum Movement
    {
        Static,
        Dynamic
    }

    public PickupType pType;
    public Movement moveType;
    Rigidbody2D rigi;
    bool stopedMoving;
    public override void startBehaviours()
    {
        base.startBehaviours();
        stopedMoving = false;
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
        moveType = t;
        if (t == Movement.Dynamic)
            rigi.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rigi.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag != TagManager.Enemy)
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

        rigi.AddForce(v * 50f);
    }
}
