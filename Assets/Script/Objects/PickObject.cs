using UnityEngine;
using System.Collections;

public class PickObject : BaseObject
{

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

    void Awake()
    {
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
        switch(collision.transform.tag)
        {
            case TagManager.Player:
                StartCoroutine(fadeOut(0.1f, 0));
                break;

            case TagManager.Ground:
                stopedMoving = true;
                break;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.playerControler || !stopedMoving)
            return;

        Vector2 v;
        Vector2 d = GameManager.playerControler.transform.position-transform.position;
        v = util.MathHelper.AngleToVector(util.MathHelper.VectorToAngle(d));

        rigi.AddForce( v * 50f);
    }
}
