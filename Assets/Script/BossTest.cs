using UnityEngine;
using System.Collections;

public class BossTest : MonoBehaviour {
    Vector3 startSize, FinalSize;
    float l;
    void Start()
    {
        BaseObject.onHitBose += hit;
        FinalSize = transform.localScale;
        transform.localScale /= 30f;
        startSize = transform.localScale;

    }

    void Destory()
    {
        BaseObject.onHitBose -= hit;
    }

	void hit(BaseObject.objectType t)
    {
        l += 1f / 30;
        if (l > 1)
            l = 1;
        transform.localScale = Vector3.Lerp(startSize, FinalSize, l);
    }
}
