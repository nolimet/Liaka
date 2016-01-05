using UnityEngine;
using System.Collections;

public class BulletObject : BaseObject {

    public override void startBehaviours()
    {
        base.startBehaviours();
        ri.isKinematic = false;
        noCoroutines = true;
        a.SetTrigger("start");
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        s = collision.transform.tag;
        switch (s)
        {
            case TagManager.Enemy:
                StartCoroutine(fadeOut(1f / 30f, 1f / 30f * 3f));
                a.SetTrigger("impact");
                ri.isKinematic = true;
                break;

            case TagManager.Player:
                StartCoroutine(fadeOut(1f / 30f, 1f / 30f * 3f));
                a.SetTrigger("impact");
                ri.isKinematic = true;
                break;

            case TagManager.Boss:
                StartCoroutine(fadeOut(1f / 30f, 1f / 30f * 3f));
                a.SetTrigger("impact");
                ri.isKinematic = true;
                break;

            case TagManager.Ground:
                if (fadeWhenGroundhit)
                    StartCoroutine(fadeOut(1f / 30f, 1f / 30f * 3f));
                a.SetTrigger("impact");
                ri.isKinematic = true;
                break;
        }
    }
}
