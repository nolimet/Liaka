using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class EnemyBase : BaseObject{

    new public static event objectTypeDelegate onHitPlayer;

    public int Health, maxHealth;
    

    struct moveSet
    {
        float Duration;
        Vector2 Direction;
    }

    moveSet[] MovementPatern;

	public enum Enemytype
    {
        walking =0,
        flying =1,
        flying_high=2
    }

    public Enemytype etype;

    /// <summary>
    /// goes through all the movement sets for a enemy
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator MoveBehaviourLoop()
    {
        yield return new WaitForEndOfFrame();
    }

    public override void RemoveFromView()
    {
        unregisterDelegates();
        EnemyPool.RemoveObject(this);
    }

    protected override void dropLoot()
    {
        PickupBase p = PickupPool.GetObject(PickupBase.PickupType.Coin);
        placeObject(p);
        p = null;
        for (int i = 0; i < 5; i++)
        {
            p = GameManager.dropTable.getRandomItem();
            if (p != null)
            {
                placeObject(p);
            }
            p = null;
        }
    }

    protected void placeObject(PickupBase p)
    {
        if (p.moveType == PickupBase.Movement.Dynamic)
        {
            p.setVelocity(new Vector2(Random.Range(-5f, 5f), 3f));
            p.transform.position = transform.position;
        }
        else if (p.moveType == PickupBase.Movement.Static)
        {
            p.transform.position = transform.position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
            p.setVelocity(new Vector3(-5, 0));
        }
        p.Unstuck();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.tag == TagManager.Player)
            onHitPlayer(type);
    }
}
