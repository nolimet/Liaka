using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BasePool : MonoBehaviour
{
    public static BasePool instance;
    public delegate void ObjectRemoved(BaseObject objectRemoved);
    public event ObjectRemoved onRemove;

    [SerializeField]
    List<BaseObject> ActivePool, InActivePool;

    bool autoCollectEnemiesOnStart = true;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        ActivePool = new List<BaseObject>();
        InActivePool = new List<BaseObject>();
    }

    void Start()
    {
        if (autoCollectEnemiesOnStart)
        {
            BaseObject[] pl = FindObjectsOfType<BaseObject>();
            foreach (BaseObject p in pl)
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
        RemoveAll();
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
            RemoveObject(instance.ActivePool[i]);
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
            RemoveObject(instance.ActivePool[i]);
        }

        for (int i = instance.InActivePool.Count - 1; i >= 0; i--)
        {
            Destroy(instance.InActivePool[i].gameObject, 0.1f);
        }

        instance.InActivePool = new List<BaseObject>();
        System.GC.Collect();
    }

    public static void RemoveObject(BaseObject e)
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
            }

            if (instance.onRemove != null)
                instance.onRemove(e);
        }
    }

    public static BaseObject GetObject(BaseObject.objectType Type)
    {
        if (instance)
        {
            BaseObject e;
            if (instance.InActivePool.Any(i => i.type == Type))
            {
                e = instance.InActivePool.First(i => i.type == Type);
                instance.InActivePool.Remove(e);
                instance.ActivePool.Add(e);
                e.gameObject.SetActive(true);
            }
            else
            {
                GameObject g = Instantiate(Resources.Load("Object/" + Type.ToString()), Vector3.zero, Quaternion.identity) as GameObject;
                g.name = Type.ToString() + " - " + (instance.ActivePool.Count + instance.InActivePool.Count);
                e = g.GetComponent<BaseObject>();

                instance.ActivePool.Add(e);
                e.transform.SetParent(instance.transform, false);
            }

            e.gameObject.SendMessage("startBehaviours", SendMessageOptions.DontRequireReceiver);
            return e;
        }
        return null;
    }

    public static PickObject GetPickup(PickObject.pickupType Type)
    {
        if (instance)
        {
            PickObject e;
            if (instance.InActivePool.Any(i => i.type == BaseObject.objectType.Pickup && ((PickObject)i).TypePickup == Type))
            {
                e = (PickObject)instance.InActivePool.First(i => i.type == BaseObject.objectType.Pickup && ((PickObject)i).TypePickup == Type);
                instance.InActivePool.Remove(e);
                instance.ActivePool.Add(e);
                e.gameObject.SetActive(true);
            }
            else
            {
                GameObject g = Instantiate(Resources.Load("Object/Pickup/" + Type.ToString()), Vector3.zero, Quaternion.identity) as GameObject;
                g.name = Type.ToString() + " - " + (instance.ActivePool.Count + instance.InActivePool.Count);
                e = g.GetComponent<PickObject>();

                instance.ActivePool.Add(e);
                e.transform.SetParent(instance.transform, false);
            }

            e.gameObject.SendMessage("startBehaviours", SendMessageOptions.DontRequireReceiver);
            return e;
        }
        return null;
    }
}
