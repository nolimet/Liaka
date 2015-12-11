using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using util;

public class VariationLayerControler : LayerControler
{

    public float offSetX = 0;
    protected override void Start()
    {
        
        WorldScreenSize = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 0));

        subObjectWorldSize = transform.GetChild(0).getChildBounds().size;
        SubObjects = new List<Transform>();
        float l = offSetX;
        Transform tx;
        for (int i = 0; i < transform.childCount; i++)
        {
            tx = transform.GetChild(i);
                SubObjects.Add(tx);
            tx.localPosition = SubObjects[0].localPosition;
            tx.position = new Vector3(l, tx.position.y, tx.position.z);
                l += tx.getChildBounds().size.x;
                if (i >= 3)
                    tx.gameObject.SetActive(false);
        }
    }
    [SerializeField]
    protected List<Transform> randomSelection = null;
    protected List<Transform> negativeSelectoin = null;
    public override bool InView(Transform t)
    {
        
        //bool b = base.OutOfView(t);
        if (t.position.x +(t.getChildBounds().size.x) < WorldScreenSize.x)
        {
            randomSelection = SubObjects.Where(i => i.gameObject.activeSelf == false).ToList();
            negativeSelectoin = SubObjects.Where(i => i.gameObject.activeSelf == true).ToList();

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


            randomSelection = null;
            negativeSelectoin = null;
            return false;

        }
        return true;
    }
}
