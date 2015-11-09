using UnityEngine;
using System.Collections;

public class FlyingEnemy: EnemyBase
{
    bool running;

    Vector3 startingPoint;

    [SerializeField]
    Vector3 deb = Vector3.zero;

    [System.Serializable]
    struct movePatern
    {
        public Vector2 Dir;
        public float duration;
    }

    [SerializeField]
    movePatern[] movingPaterns;

    public override void startBehaviours()
    {
        base.startBehaviours();

        startingPoint = transform.position;
        StartCoroutine(MoveBehaviourLoop());
        
    }

    protected override IEnumerator MoveBehaviourLoop()
    {
        if (running)
            yield break;
        running = true;

        float t1 = 0, t2 = 0, tM = 0; //timer and Max time
        Vector2 p;
        int current = 0;

        current = Random.Range(0, movingPaterns.Length);
        t1 = t2 = tM = movingPaterns[current].duration;


        while (alive)
        {
            if (!paused)
            {
                p = transform.position;

                if (t1 <= 0 && t2 <= 0)
                {
                    current = Random.Range(0, movingPaterns.Length);
                    t1 = t2 = tM = movingPaterns[current].duration;
                }

                if (t1 > 0)
                {
                    p.y = Mathf.Lerp(startingPoint.y + movingPaterns[current].Dir.y, startingPoint.y, t1 / tM);
                    t1 -= Time.deltaTime;
                }
                else if (t2 > 0)
                {
                    p.y = Mathf.Lerp(startingPoint.y, startingPoint.y + movingPaterns[current].Dir.y, t2 / tM);
                    t2 -= Time.deltaTime;
                }

                transform.position = p;
                deb = p;
                Debug.Log("running " + name + " t1 = " + t1.ToString() + " t2 = " + t2.ToString() + " tm = " + tM.ToString() + " pos: " + transform.position, this.gameObject);
            }
            yield return new WaitForEndOfFrame();
        }

        running = false;
    }

    protected override IEnumerator fadeOut(float f, float d, string g = "", bool DisableCollsionAtStart = true)
    {
        ri.isKinematic = true;
        return base.fadeOut(f, d, g, DisableCollsionAtStart);
    }
}
