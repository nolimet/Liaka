using UnityEngine;
using System.Collections;

public class PlayerGun : MonoBehaviour {

    public Transform fireLocation;

    public void Shoot(Vector2 v2)
    {
        if (!GameManager.stageControler.bossFighting && !GameManager.gamePaused)
        {
            Debug.Log("Shoot");
            Vector3 wp = Camera.main.ScreenToWorldPoint(new Vector3(v2.x, v2.y, 0));
            Vector3 d = wp - transform.position;
            Quaternion q = Quaternion.Euler(0, 0, util.MathHelper.VectorToAngle(d));
            transform.rotation = q;

            BaseObject b = BasePool.GetObject(BaseObject.objectType.Bullet);

            b.transform.rotation = q;
            b.transform.position = fireLocation.position;

            b.setVelocity(util.MathHelper.AngleToVector(util.MathHelper.VectorToAngle(d)) * 20f);
        }
    }
}
