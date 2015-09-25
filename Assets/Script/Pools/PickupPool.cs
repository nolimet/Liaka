using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PickupPool : MonoBehaviour
{

    public static PickupPool instance;
    public delegate void ObjectRemoved(PickupBase objectRemoved);
    public event ObjectRemoved onRemove;

    List<PickupBase> ActivePool, InActivePool;

    bool autoCollectEnemiesOnStart = true;

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
        if (autoCollectEnemiesOnStart)
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
        for (int i = ActivePool.Count - 1; i >= 0; i--)
        {
            RemoveObject(ActivePool[i]);
        }
    }

    public static void RemoveObject(PickupBase e)
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
                e.Instance_onPauseGame(true);
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
                GameObject g = Instantiate(Resources.Load("Object/Pickup/" + Type.ToString()), Vector3.zero, Quaternion.identity) as GameObject;
                if (g == null)
                    return null;
                g.name = Type.ToString() + " - " + (instance.ActivePool.Count + instance.InActivePool.Count);
                e = g.GetComponent<PickupBase>();

                instance.ActivePool.Add(e);
                e.transform.SetParent(instance.transform, false);
            }

            e.gameObject.SendMessage("startBehaviours", SendMessageOptions.DontRequireReceiver);
            return e;
        }
        return null;
    }
}
