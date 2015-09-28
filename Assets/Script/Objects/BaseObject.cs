using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D))]
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
    Vector2 Speed = Vector2.zero;

    bool delegateSet = false;
    bool fading = false;


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
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        if (r)
            r.color = new Color(r.color.r, r.color.g, r.color.b, 1);

        CircleCollider2D cp = GetComponent<CircleCollider2D>();
        if (cp)
            cp.enabled = true;
        
        Rigidbody2D ri = GetComponent<Rigidbody2D>();
        if (ri)
            ri.isKinematic = false;

        if (type == objectType.Enemy)
        {
            setVelocity(new Vector3(-5, 0));
        }

        if (!delegateSet)
        {
            delegateSet = true;
            GameManager.instance.onPauseGame += Instance_onPauseGame;
        }

        StopAllCoroutines();
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

    /// <summary>
    /// Throw object out of the active object pool
    /// </summary>
    public virtual void RemoveFromView()
    {
        if (delegateSet)
        {
            delegateSet = false;
            GameManager.instance.onPauseGame -= Instance_onPauseGame;
        }
        BasePool.RemoveObject(this);
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {

        string s = collision.transform.tag;
        switch (s)
        {
            case TagManager.Enemy:
                collision.gameObject.SendMessage("hit",tag ,SendMessageOptions.DontRequireReceiver);
                StartCoroutine(fadeOut(0.1f, 0f));

                if (onImpact != null)
                    onImpact(type);
                break;

            case TagManager.Player:
                StartCoroutine(fadeOut(0.1f, 0));

                if (onHitPlayer != null)
                    onHitPlayer(type);
                break;

            case TagManager.Boss:
                StartCoroutine(fadeOut(0.2f, 0));
                

                if (onHitBose != null)
                    onHitBose(type);
                break;

            case TagManager.Ground:
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
    /// <returns></returns>
    protected virtual IEnumerator fadeOut(float f, float d, string g = "")
    {
        if (!fading)
        {
            if (type == objectType.Enemy && g == TagManager.Bullet)
                dropLoot();  
            fading = true;
            
            float z = f;
            SpriteRenderer r = GetComponent<SpriteRenderer>();
            GetComponent<CircleCollider2D>().enabled = false;

            Color c = r.color;

            yield return new WaitForSeconds(d);
            while (z > 0)
            {
                c.a = z / f;
                r.color = c;
                z -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            fading = false;
            RemoveFromView();
        }
    }

    public virtual void Instance_onPauseGame(bool b)
    {
        Rigidbody2D r = GetComponent<Rigidbody2D>();
        if(b)
        {
            Speed = r.velocity;
            r.velocity = Vector2.zero;
            r.isKinematic = true;
        }
        else
        {
            r.isKinematic = false;
            r.velocity = Speed;
        }
    }

    protected virtual void dropLoot()
    {
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
        }
    }

    protected virtual void Update()
    {
        if (transform.position.x > 100)
            RemoveFromView();
    }
}
