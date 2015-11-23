using UnityEngine;
using System.Collections;

public class PlayerGun : MonoBehaviour {

    public Transform fireLocation;

    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    const float maxAngle = 15f;

    public void Shoot(Vector2 v2)
    {
        if (!GameManager.stageControler.bossFighting && !GameManager.gamePaused)
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(new Vector3(v2.x, v2.y, 0));
            Vector3 d = wp - transform.position;

            float a = util.MathHelper.VectorToAngle(d);

            if (a > maxAngle)
                a = maxAngle;
            else if (a < -maxAngle)
                a = -maxAngle;
           
            Quaternion q = Quaternion.Euler(0, 0, a);
            transform.rotation = q;

            BaseObject b = BasePool.GetObject(BaseObject.objectType.Bullet);

            b.transform.rotation = q;
            b.transform.position = fireLocation.position;

            b.setVelocity(util.MathHelper.AngleToVector(a) * 20f);
        }
    }
}
