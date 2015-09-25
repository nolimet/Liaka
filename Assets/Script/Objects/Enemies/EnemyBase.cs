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

        PickupBase p = GameManager.dropTable.getRandomItem();
        for (int i = 0; i < 5; i++)
        {
            if (p != null)
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
    }
}
