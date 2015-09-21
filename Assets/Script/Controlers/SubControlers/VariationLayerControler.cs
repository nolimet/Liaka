using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VariationLayerControler : LayerControler
{
    protected override void Start()
    {
        Transform[] tx = GetComponentsInChildren<Transform>();
        WorldScreenSize = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 0));
        SubObjects = new List<Transform>();

        for (int i = 0; i < tx.Length; i++)
        {
            if (tx[i] != transform)
            {
                SubObjects.Add(tx[i]);
                tx[i].position = new Vector3(subObjectWorldSize.x * (i - 1), tx[i].position.y);
                if (i > 3)
                    tx[i].gameObject.SetActive(false);
            }
        }
    }

    public override bool OutOfView(Transform t)
    {
        //bool b = base.OutOfView(t);
        if ((t.position + (subObjectWorldSize / 2f)).x < WorldScreenSize.x)
        {
            t.position += new Vector3(subObjectWorldSize.x * 3, 0);

            List<Transform> randomSelection = SubObjects.Where(i => i.gameObject.activeSelf == false).ToList();
            Transform freeObject = randomSelection.ElementAtOrDefault(new System.Random().Next() % randomSelection.Count());
            t.gameObject.SetActive(false);
            freeObject.gameObject.SetActive(true);
            freeObject.position = t.position;
            return true;

        }
        return false;
    }
}
