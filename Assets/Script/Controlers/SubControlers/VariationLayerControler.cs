using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using util;

public class VariationLayerControler : LayerControler
{
    protected override void Start()
    {
        
        WorldScreenSize = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 0));
        subObjectWorldSize = transform.GetChild(0).getChildBounds().size;
        SubObjects = new List<Transform>();
        float l = 0f;
        Debug.Log(l);
        Transform tx;
        for (int i = 0; i < transform.childCount; i++)
        {
            tx = transform.GetChild(i);
                SubObjects.Add(tx);
                tx.position = new Vector3(l, tx.position.y);
                l += tx.getChildBounds().size.x;
                if (i >= 3)
                    tx.gameObject.SetActive(false);
        }
    }

    public override bool OutOfView(Transform t)
    {
        
        //bool b = base.OutOfView(t);
        if (t.position.x +(t.getChildBounds().size.x) < WorldScreenSize.x)
        {
            List<Transform> randomSelection = SubObjects.Where(i => i.gameObject.activeSelf == false).ToList();
            List<Transform> negativeSelectoin = SubObjects.Where(i => i.gameObject.activeSelf == true).ToList();

            Transform f = transform;
            foreach (Transform x in negativeSelectoin)
                if (x.transform.position.x > f.position.x)
                    f = x;
            
            if (randomSelection.Count > 0)
            {
                Transform freeObject = randomSelection.ElementAtOrDefault(new System.Random().Next() % randomSelection.Count());
                t.gameObject.SetActive(false);
                freeObject.gameObject.SetActive(true);

                freeObject.position = f.position + new Vector3(f.getChildBounds().size.x, 0, 0);
            }
            return true;

        }
        return false;
    }
}
