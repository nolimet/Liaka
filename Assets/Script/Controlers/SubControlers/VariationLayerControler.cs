using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using util;

public class VariationLayerControler : LayerControler
{

    public float offSetX = 0;
    public string[] IgnorNameTags;
    protected override void Start()
    {
        if (GameManager.instance)
            WorldScreenSize = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 0));

        

        //subObjectWorldSize = transform.GetChild(0).getChildBounds(IgnorNameTags).size;
        SubObjects = new List<Transform>();
        float l = offSetX;
        Transform tx;
        for (int i = 0; i < transform.childCount; i++)
        {
            tx = transform.GetChild(i);
                SubObjects.Add(tx);
            tx.localPosition = SubObjects[0].localPosition;
            tx.position = new Vector3(l, tx.position.y, tx.position.z);
                l += tx.getChildBounds(IgnorNameTags).size.x;
                if (i >= 3)
                    tx.gameObject.SetActive(false);
        }
    }

    protected List<Transform> randomSelection = null;
    protected List<Transform> negativeSelectoin = null;
    public override bool InView(Transform t)
    {
        
        //bool b = base.OutOfView(t);
        if ((t.getChildBounds().max.x) < WorldScreenSize.x)
        {
            t.gameObject.SetActive(false);
            randomSelection = SubObjects.Where(i => i.gameObject.activeSelf == false).ToList();
            negativeSelectoin = SubObjects.Where(i => i.gameObject.activeSelf == true).ToList();

            Transform f =t;
            foreach (Transform x in negativeSelectoin)
                if (x.transform.position.x > f.position.x)
                    f = x;
           
            if (randomSelection.Count > 0)
            {
                Transform freeObject = randomSelection.ElementAtOrDefault(new System.Random().Next() % randomSelection.Count());
                freeObject.gameObject.SetActive(true);
                if (IgnorNameTags == null)
                    freeObject.position = f.position + new Vector3(f.getChildBounds().size.x, 0, 0);
                else
                    freeObject.position = f.position + new Vector3(f.getChildBounds(IgnorNameTags).size.x, 0, 0);

                util.Debugger.Log("bound orignal " + f.name, f.getChildBounds(IgnorNameTags));
                util.Debugger.Log("bound newObject " + freeObject.name, freeObject.getChildBounds(IgnorNameTags));
            }

            randomSelection = null;
            negativeSelectoin = null;
            return false;

        }
        return true;
    }

    public override void LateUpdateLoop()
    {
        base.LateUpdateLoop();
       //drawOutLine();
    }
    Bounds b;
    public virtual void drawOutLine()
    {
        
        foreach(Transform t in SubObjects)
        {
            b = t.getChildBounds(IgnorNameTags);
            
            util.Common.DrawBounds(b);

            //b = t.getChildBounds();
            //Debug.DrawLine(b.max, new Vector3(b.max.x, b.min.y), Color.red);
            //Debug.DrawLine(new Vector3(b.max.x, b.min.y), b.min, Color.red);
            //Debug.DrawLine(b.min, new Vector3(b.min.x, b.max.y), Color.red);
            //Debug.DrawLine(new Vector3(b.min.x, b.max.y), b.max, Color.red);
        }
    }
}
