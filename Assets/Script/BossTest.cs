using UnityEngine;
using System.Collections;

public class BossTest : MonoBehaviour {

    void Start()
    {
        BaseObject.onHitBose += hit;
    }

    void Destory()
    {
        BaseObject.onHitBose -= hit;
    }

	void hit(BaseObject.objectType t)
    {
        GetComponent<SpriteRenderer>().color += Color.red / 10f;
    }
}
