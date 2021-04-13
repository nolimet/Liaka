using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BaseObject : MonoBehaviour
{
    public delegate void objectTypeDelegate(objectType o);

    public static event objectTypeDelegate onImpact, onHitBose, onHitPlayer;

    public enum objectType
    {
        Pickup,
        Bullet,
        Enemy,
        Default
    }

    [System.Serializable]
    public struct bounds
    {
        [Tooltip("true means it will use that bound. False means it will be ignored")]
        public bool up, down, left, right;

        /// <summary>
        /// check if object is touching one of the bounds
        /// </summary>
        /// <param name="b">name of the bound in anyformat</param>
        /// <returns>value based on the fact of the object is touching one of the bounds</returns>
        public bool touchingBound(string b)
        {
            b = b.ToLower();

            if (b.Contains("up") && up)
                return true;
            if (b.Contains("down") && down)
                return true;
            if (b.Contains("left") && left)
                return true;
            if (b.Contains("right") && right)
                return true;

            return false;
        }
    }

    [SerializeField]
    protected bounds useBounds;

    public objectType type;
    private Vector2 Speed = Vector2.zero;

    protected bool delegateSet = false;
    protected bool fading = false;
    protected bool alive = false;
    protected bool paused = false;
    protected bool noCoroutines = false;
    public bool fadeWhenGroundhit = true;
    public bool removeObject = false;

    protected float OriginalTimeScale = 0f; // timescale of SkeletonAnimation

    protected Rigidbody2D ri;
    protected Animator a;
    protected CircleCollider2D cp;
    protected SpriteRenderer sr;
    protected SkeletonAnimation SA;

    protected virtual void Awake()
    {
        if (!sr)
            sr = GetComponent<SpriteRenderer>();
        if (!cp)
            cp = GetComponent<CircleCollider2D>();
        if (!a)
            a = GetComponent<Animator>();
        if (!ri)
            ri = GetComponent<Rigidbody2D>();
        if (!SA)
            SA = GetComponentInChildren<SkeletonAnimation>();

        if (SA)
            OriginalTimeScale = SA.timeScale;
    }

    /// <summary>
    /// Set movement speed for the object
    /// </summary>
    /// <param name="v">Speed for each direction.</param>
    public virtual void setVelocity(Vector2 v)
    {
        GetComponent<Rigidbody2D>().velocity = v;
    }

    /// <summary>
    /// Used when placing the object into the world
    /// </summary>
    public virtual void startBehaviours()
    {
        ChangeColour(Color.white);

        if (cp)
            cp.enabled = true;
        if (a)
            a.enabled = true;
        if (ri)
            ri.isKinematic = false;

        if (!delegateSet)
        {
            delegateSet = true;
            GameManager.instance.onPauseGame += Instance_onPauseGame;
        }
        alive = true;
        //StopAllCoroutines();
    }

    /// <summary>
    /// called by sending a message signaling that the object has been hit
    /// </summary>
    /// <param name="s">Tag of the object that collided with this one</param>
    public virtual void hit(string s = "")
    {
        StartCoroutine(fadeOut(0.3f, 0, s));
        GetComponent<CircleCollider2D>().enabled = false;
    }

    public virtual void RemoveObject(float f, float d, string g = "", bool DisableCollsionAtStart = true)
    {
        StartCoroutine(fadeOut(f, d, g, DisableCollsionAtStart));
    }

    /// <summary>
    /// Throw object out of the active object pool
    /// </summary>
    public virtual void RemoveFromView(bool callRemoveOjbect = true)
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
            BasePool.RemoveObject(this);
    }

    public virtual void unregisterDelegates()
    {
        if (delegateSet)
        {
            delegateSet = false;
            GameManager.instance.onPauseGame -= Instance_onPauseGame;
        }
    }

    protected string s;

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        s = collision.transform.tag;
        switch (s)
        {
            case TagManager.Enemy:
                collision.gameObject.SendMessage("hit", tag, SendMessageOptions.DontRequireReceiver);
                if (!noCoroutines)
                    StartCoroutine(fadeOut(0.1f, 0f));

                if (onImpact != null)
                    onImpact(type);
                break;

            case TagManager.Player:
                if (!noCoroutines)
                    StartCoroutine(fadeOut(0.1f, 0));

                if (onHitPlayer != null)
                    onHitPlayer(type);
                break;

            case TagManager.Boss:
                if (!noCoroutines)
                    StartCoroutine(fadeOut(0.2f, 0));

                if (onHitBose != null)
                    onHitBose(type);
                break;

            case TagManager.Ground:
                if (fadeWhenGroundhit && !noCoroutines)
                    StartCoroutine(fadeOut(0.2f, 0));

                if (onImpact != null)
                    onImpact(type);
                break;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.transform.name);
        if (collision.transform.tag != TagManager.Enemy)
            if (collision.name.ToUpper().Contains("BOX") && useBounds.touchingBound(collision.name) || !collision.name.ToUpper().Contains("BOX") && collision.name.ToUpper().Contains("PLAYER"))
            {
                StartCoroutine(fadeOut(0.1f, 0.6f));
            }
    }

    /// <summary>
    /// fades the object out
    /// </summary>
    /// <param name="f">Time the fading takes</param>
    /// <param name="d">Starting Delay before the fading</param>
    /// <param name="g">Tag of the object that started it</param>
    /// <param name="DisableCollsionAtStart">Disable colision checking at the start of the fading Opperation</param>
    /// <returns></returns>
    protected virtual IEnumerator fadeOut(float f, float d, string g = "", bool DisableCollsionAtStart = true)
    {
        if (!fading)
        {
            alive = false;
            if (type == objectType.Enemy && g == TagManager.Bullet)
                dropLoot();
            fading = true;

            float z = f;

            if (GetComponent<Animator>())
                GetComponent<Animator>().enabled = true;

            Color c = getColour();

            if (DisableCollsionAtStart)
                GetComponent<CircleCollider2D>().enabled = false;

            yield return new WaitForSeconds(d);

            if (!DisableCollsionAtStart)
                GetComponent<CircleCollider2D>().enabled = false;

            while (z > 0)
            {
                c.a = z / f;
                ChangeColour(c);
                z -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            fading = false;
            RemoveFromView();
        }
    }

    public virtual void Instance_onPauseGame(bool b)
    {
        // Rigidbody2D r = GetComponent<Rigidbody2D>();
        //Animator a = GetComponent<Animator>();
        paused = b;

        try
        {
            if (this == null)
            {
                Debug.Log("null pointer in Pause_Game");
                GameManager.instance.onPauseGame -= Instance_onPauseGame;
                return;
            }
            if (b)
            {
                Speed = ri.velocity;
                ri.velocity = Vector2.zero;
                ri.isKinematic = true;

                if (a)
                    a.enabled = false;

                if (SA)
                    SA.timeScale = 0;
            }
            else
            {
                ri.isKinematic = false;
                ri.velocity = Speed;

                if (a)
                    a.enabled = true;

                if (SA)
                    SA.timeScale = OriginalTimeScale;
            }
        }
        catch (System.Exception)
        {
            GameManager.instance.onPauseGame -= Instance_onPauseGame;
            throw;
        }
    }

    protected virtual void dropLoot()
    {
        Debug.LogWarning("Calling old dropFunc");
        /*
        PickObject p;
        int l = Random.Range(1, 10);
        float r;
        for (int i = 0; i < l; i++)
        {
            r = Random.Range(0f, 50f);
            if (r < 5f)
            {
                p = BasePool.GetPickup(PickObject.pickupType.Dynamic_Energy);
                p.setVelocity(new Vector2(Random.Range(-5f, 5f), 3f));
                p.transform.position = transform.position;
            }
            else if (r < 10f)
            {
                p = BasePool.GetPickup(PickObject.pickupType.Dynamic_Coin);
                p.setVelocity(new Vector2(Random.Range(-5f, 5f), 3f));
                p.transform.position = transform.position;
            }
            else if (r < 20f)
            {
                p = BasePool.GetPickup(PickObject.pickupType.Static_Coin);
                p.transform.position = transform.position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                p.setVelocity(new Vector3(-5, 0));
            }
            else
            {
                p = BasePool.GetPickup(PickObject.pickupType.Static_Energy);
                p.transform.position = transform.position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                p.setVelocity(new Vector3(-5, 0));
            }
        }*/
    }

    public virtual void setPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }

    protected virtual void Update()
    {
        if (transform.position.x > 100)
            RemoveFromView();
    }

    protected void ChangeColour(Color c)
    {
        if (sr)
        {
            sr.color = c;
        }

        if (SA)
        {
            SA.skeleton.SetColor(c);
        }
    }

    protected Color getColour()
    {
        if (sr)
        {
            return sr.color;
        }

        if (SA)
        {
            return new Color(SA.skeleton.R, SA.skeleton.G, SA.skeleton.B, SA.skeleton.A);
        }

        return Color.white;
    }
}