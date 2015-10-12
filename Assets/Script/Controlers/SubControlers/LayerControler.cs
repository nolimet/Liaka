using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerControler : MonoBehaviour {
    [SerializeField]
    protected List<Transform> SubObjects;
    [SerializeField]
    protected Vector3 subObjectWorldSize;
    protected static Vector2 WorldScreenSize;

    virtual protected void Start()
    {
        Transform[] tx = GetComponentsInChildren<Transform>();
        SubObjects = new List<Transform>();
        subObjectWorldSize = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size;
        WorldScreenSize = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 0));
        

        for (int i = 0; i < tx.Length; i++)
        {
            if (tx[i] != transform)
            {
                SubObjects.Add(tx[i]);
                tx[i].position = new Vector3(subObjectWorldSize.x * (i - 1), tx[i].position.y);
            }
        }
    }

	public virtual void moveLayer(Vector3 v)
    {
        foreach(Transform t in SubObjects)
        {
            if (t.gameObject.activeSelf)
            {
                t.Translate(v * Time.deltaTime);
                OutOfView(t);
            }
        }
    }

    public virtual bool OutOfView(Transform t)
    {
        if ((t.position + (subObjectWorldSize / 2f)).x < WorldScreenSize.x)
        {
            t.position += new Vector3(subObjectWorldSize.x * (SubObjects.Count), 0);
            return true;
        }
        return false;
    }

    
}
