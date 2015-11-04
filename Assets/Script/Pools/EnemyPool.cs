using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyPool : MonoBehaviour
{

    public static EnemyPool instance;
    public delegate void ObjectRemoved(EnemyBase objectRemoved);
    public event ObjectRemoved onRemove;

    [SerializeField]
    List<EnemyBase> ActivePool, InActivePool;

    bool autoCollectEnemiesOnStart = true;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        ActivePool = new List<EnemyBase>();
        InActivePool = new List<EnemyBase>();
    }

    void Start()
    {
        if (autoCollectEnemiesOnStart)
        {
            EnemyBase[] pl = FindObjectsOfType<EnemyBase>();
            foreach (EnemyBase p in pl)
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
        DestroyAll();
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

        for(int i = instance.InActivePool.Count-1;i>=0; i--)
        {
            Destroy(instance.InActivePool[i].gameObject,0.1f);
        }

        instance.InActivePool = new List<EnemyBase>();
        System.GC.Collect();
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

    public static void RemoveObject(EnemyBase e)
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

    public static EnemyBase GetObject(EnemyBase.Enemytype Type)
    {
        if (instance)
        {
            EnemyBase e;
            if (instance.InActivePool.Any(i => i.etype == Type))
            {
                e = instance.InActivePool.First(i => i.etype == Type);
                instance.InActivePool.Remove(e);
                instance.ActivePool.Add(e);
                e.gameObject.SetActive(true);
            }
            else
            {
                GameObject g = Instantiate(Resources.Load("Object/Enemies/" + Application.loadedLevelName + "/" + Type.ToString()), Vector3.zero, Quaternion.identity) as GameObject;
                if(!g)
                    g = Instantiate(Resources.Load("Object/Enemies/" + Type.ToString()), Vector3.zero, Quaternion.identity) as GameObject;
                g.name = Type.ToString() + " - " + (instance.ActivePool.Count + instance.InActivePool.Count);
                e = g.GetComponent<EnemyBase>();

                instance.ActivePool.Add(e);
                e.transform.SetParent(instance.transform, false);
            }

            e.gameObject.SendMessage("startBehaviours", SendMessageOptions.DontRequireReceiver);
            return e;
        }
        return null;
    }
}
