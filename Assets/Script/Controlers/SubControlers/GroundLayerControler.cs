using UnityEngine;
using System.Collections;
using System.Linq;
using util;

public class GroundLayerControler : VariationLayerControler
{

    public float disableTraps;

    protected override void Start()
    {
        base.Start();
        
        if (disableTraps <= 0)
            return;

        int activeBits = 0;
        foreach (Transform t in SubObjects)
        {
            if (t.name.ToLower().Contains("trap"))
                t.gameObject.SetActive(false);

            if (t.gameObject.activeSelf)
                activeBits++;
        }

        if (activeBits < 3)
        {
            if (disableTraps > 0)
                randomSelection = SubObjects.Where(i => i.gameObject.activeSelf == false && !i.name.ToLower().Contains("trap")).ToList();
            else
                randomSelection = SubObjects.Where(i => i.gameObject.activeSelf == false).ToList();

            negativeSelectoin = SubObjects.Where(i => i.gameObject.activeSelf == true).ToList();

            Transform f = transform;
            if (negativeSelectoin.Count > 1)
            {
                foreach (Transform x in negativeSelectoin)
                    if (x.transform.position.x > f.position.x)
                        f = x;
            }
            else
                if (negativeSelectoin.Count > 0)
                f = negativeSelectoin[0];


                foreach (Transform t in SubObjects)
                {
                    if (activeBits < 3)
                        if (!t.name.ToLower().Contains("trap") && !t.gameObject.activeSelf)
                        {
                            t.gameObject.SetActive(true);
                            activeBits++;
                            t.position =  new Vector3(f.getChildBounds("tree").max.x, f.position.y, 0);
                            f = t;
                        }
                }
        }
    }

    public override void LateUpdateLoop()
    {
        base.LateUpdateLoop();
        if (disableTraps > 0)
            disableTraps -= Time.deltaTime;
    }

    public override bool InView(Transform t)
    {
        //bool b = base.OutOfView(t);
        if (t.position.x + (t.getChildBounds("tree").size.x) < WorldScreenSize.x)
        {
            if (disableTraps > 0)
                randomSelection = SubObjects.Where(i => i.gameObject.activeSelf == false && !i.name.ToLower().Contains("trap")).ToList();
            else
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

                freeObject.position =  new Vector3(f.getChildBounds("tree").max.x, f.position.y, 0);
            }


            randomSelection = null;
            negativeSelectoin = null;
            return false;

        }
        return true;
    }
}
