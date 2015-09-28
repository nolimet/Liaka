using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class EnemyBase : BaseObject{

    public int Health, maxHealth;

    struct moveSet
    {
        float Duration;
        Vector2 Direction;
    }

    moveSet[] MovementPatern;

	public enum Enemytype
    {
        walking,
        flying
    }

    public Enemytype etype;

    public virtual void Start()
    {

    }

    /// <summary>
    /// goes through all the movement sets for a enemy
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator MoveBehaviourLoop()
    {
        yield return new WaitForEndOfFrame();
    }

    protected override void dropLoot()
    {
        //base.dropLoot();

        PickupBase p = PickupPool.GetObject(PickupBase.PickupType.Coin);
        placeObject(p);

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
    }
}
