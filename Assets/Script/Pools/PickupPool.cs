﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PickupPool : MonoBehaviour
{

    public static PickupPool instance;
    public delegate void ObjectRemoved(PickupBase objectRemoved);
    public event ObjectRemoved onRemove;

    [SerializeField]
    List<PickupBase> ActivePool, InActivePool;

    [Tooltip("will it auto collect all objects of it's own type")]
    bool AutoCollectObjects = true;

    public int activeObjects
    {
        get
        {
            return ActivePool.Count();
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        ActivePool = new List<PickupBase>();
        InActivePool = new List<PickupBase>();
    }

    void Start()
    {
        if (AutoCollectObjects)
        {
            PickupBase[] pl = FindObjectsOfType<PickupBase>();
            foreach (PickupBase p in pl)
            {
                if (p.gameObject.activeSelf)
                    ActivePool.Add(p);
                else
                    InActivePool.Add(p);

                p.transform.SetParent(transform);
            }
        }
    }

    void Update()
    {
        if (instance == null)
            instance = this;
    }

    public void OnLevelWasLoaded(int level)
    {
        //DestroyAll();
    }

    /// <summary>
    /// Removes all objects from view Immediately
    /// </summary>
    public static void RemoveAllImmediate()
    {
        if (!instance)
            return;

        for (int i = instance.ActivePool.Count - 1; i >= 0; i--)
        {
            RemoveObject(instance.ActivePool[i]);
        }
    }

    /// <summary>
    /// Removes all objects from view
    /// </summary>
    public static void RemoveAll()
    {
        if (!instance)
            return;

        for (int i = instance.ActivePool.Count - 1; i >= 0; i--)
        {
            instance.ActivePool[i].hit();
        }
    }

    /// <summary>
    /// Destory's all objects in pool from scene
    /// </summary>
    public static void DestroyAll()
    {
        if (!instance)
            return;

        for (int i = instance.ActivePool.Count - 1; i >= 0; i--)
        {
            RemoveObject(instance.ActivePool[i],true);
        }

        for (int i = instance.InActivePool.Count - 1; i >= 0; i--)
        {
            Destroy(instance.InActivePool[i].gameObject, 0.1f);
        }

        instance.InActivePool = new List<PickupBase>();
        instance.ActivePool = new List<PickupBase>();
        System.GC.Collect();
    }

    public static void RemoveObject(PickupBase e, bool disableEvents = false)
    {
        if (instance)
        {
            if (instance.ActivePool.Contains(e))
                instance.ActivePool.Remove(e);

            if (!instance.InActivePool.Contains(e))
                instance.InActivePool.Add(e);

            if (e)
            {
                e.transform.position = new Vector3(0, 500, -50);
                e.RemoveFromView(false);
                if (disableEvents)
                    e.unregisterDelegates();
            }

            if (instance.onRemove != null)
                instance.onRemove(e);
        }
    }

    public static PickupBase GetObject(PickupBase.PickupType Type)
    {
        if (instance)
        {
            PickupBase e;
            if (instance.InActivePool.Any(i => i.pType == Type))
            {
                e = instance.InActivePool.First(i => i.pType == Type);
                instance.InActivePool.Remove(e);
                instance.ActivePool.Add(e);
                e.gameObject.SetActive(true);
            }
            else
            {
                //stage Specific location
               // GameObject g = Instantiate(Resources.Load("Object/Pickup/" + Application.loadedLevelName + "/"  + Type.ToString()), Vector3.zero, Quaternion.identity) as GameObject;
                //None Stage Specific
                GameObject g = Instantiate(Resources.Load("Object/Pickup/" + Type.ToString()), Vector3.zero, Quaternion.identity) as GameObject;
                if (g == null)
                    return null;
                g.name = Type.ToString() + " - " + (instance.ActivePool.Count + instance.InActivePool.Count);
                e = g.GetComponent<PickupBase>();

                instance.ActivePool.Add(e);
                e.transform.SetParent(instance.transform, false);
            }

            e.gameObject.SendMessage("startBehaviours");
            return e;
        }
        return null;
    }
}
