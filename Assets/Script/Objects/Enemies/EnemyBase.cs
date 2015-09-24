using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour {

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

    public Enemytype type;

    public virtual void Start()
    {

    }

    public virtual void StartBehaviour()
    {

    }

    protected virtual IEnumerator MoveBehaviourLoop()
    {
        yield return new WaitForEndOfFrame();
    }

    public virtual void Instance_onPauseGame(bool state)
    {

    }
}
