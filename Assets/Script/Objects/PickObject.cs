using UnityEngine;
using System.Collections;

public class PickObject : BaseObject
{
    public delegate void PickupObjectEvent(pickupType p);
    public static event PickupObjectEvent onPickup;

    public pickupType TypePickup;
    Rigidbody2D rigi;
    bool stopedMoving;
    public enum pickupType
    {
        Static_Coin,
        Dynamic_Coin,
        Static_Energy,
        Dynamic_Energy
    }

    protected override void Awake()
    {
        base.Awake();

        type = objectType.Pickup;
        rigi = GetComponent<Rigidbody2D>();
    }

    public override void startBehaviours()
    {
        base.startBehaviours();
        stopedMoving = false;
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        //base.OnCollisionEnter2D(collision);

        switch (collision.transform.tag)
        {
            case TagManager.Ground:
                stopedMoving = true;
                break;
        }

    }

    

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        //base.OnTriggerEnter2D(collision);
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
                    onPickup(TypePickup);
                break;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.playerControler || !stopedMoving)
            return;

        Vector2 v;
        Vector2 d = GameManager.playerControler.transform.position - transform.position;
        v = util.Common.AngleToVector(util.Common.VectorToAngle(d));

        rigi.AddForce(v * 50f);
    }
}
