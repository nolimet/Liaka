using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerControler : MonoBehaviour {
    [SerializeField]
    protected List<Transform> SubObjects;
    [SerializeField]
    protected Vector3 subObjectWorldSize;
    protected static Vector2 WorldScreenSize;

    protected Vector3 StartPos;

    public bool IgnorShake = true;
    public bool DoUpdate = true;
    void Awake()
    {
        StartPos = transform.localPosition;
    }

    virtual protected void Start()
    {
        Transform[] tx = GetComponentsInChildren<Transform>();
        SubObjects = new List<Transform>();
        subObjectWorldSize = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size;
        WorldScreenSize = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 0));

        if (transform.childCount == 0)
            return;

        for (int i = 0; i < tx.Length; i++)
        {
            if (tx[i] != transform)
            {
                SubObjects.Add(tx[i]);
                tx[i].position = new Vector3(subObjectWorldSize.x * (i - 1), tx[i].position.y, tx[i].position.z);
            }
        }
    }

	public virtual void moveLayer(Vector3 v)
    {
        if (SubObjects.Count == 0 || !DoUpdate)
            return;

        foreach (Transform t in SubObjects)
        {
            if (t.gameObject.activeSelf)
                InView(t);
        }
        foreach(Transform t in SubObjects)
        {
            if (t.gameObject.activeSelf)
            {
                t.Translate(v * Time.deltaTime);
            }
        }
    }

    public virtual bool InView(Transform t)
    {
        if ((t.position + (subObjectWorldSize / 2f)).x < WorldScreenSize.x)
        {
            t.position += new Vector3(subObjectWorldSize.x * (SubObjects.Count), 0);
            return false;
        }
        return true;
    }

    public virtual void FixedLayerMove(Vector3 v) { }
    
    public virtual void LateUpdateLoop()
    {
        if (IgnorShake)
        {
            if (!ScreenShaker.Shaking)
                transform.localPosition = StartPos;
            else
                transform.localPosition = StartPos + (Vector3)ScreenShaker.currenOffSet;
        }
    }
}
