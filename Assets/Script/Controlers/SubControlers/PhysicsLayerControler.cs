using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PhysicsLayerControler : VariationLayerControler
{
    List<Rigidbody2D> rigiBodies;
    protected override void Start()
    {
        base.Start();

        rigiBodies = new List<Rigidbody2D>();

       foreach(Transform t in SubObjects)
            rigiBodies.Add(t.gameObject.AddComponent<Rigidbody2D>());

        foreach (Rigidbody2D r in rigiBodies)
        {
            r.isKinematic = true;
            r.constraints= RigidbodyConstraints2D.FreezeAll;
        }
    }

    public override void moveLayer(Vector3 v)
    {
        
    }

    public override void FixedLayerMove(Vector3 v)
    {
        Transform t;
        for (int i = 0; i < SubObjects.Count; i++)
        {
            t = SubObjects[i];
            if (t.gameObject.activeSelf)
            {
                rigiBodies[i].MovePosition(t.position + (v * Time.fixedDeltaTime));
                //t.Translate(v * Time.deltaTime);
                OutOfView(t);
            }
        }
    }
}
