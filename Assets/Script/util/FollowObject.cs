using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

    public bool followOnX;
    public bool followOnY;

    public Transform target;
    void Update()
    {
        Vector3 p = transform.position;
        if (followOnX)
            p.x = target.position.x;

        if (followOnY)
            p.y = target.position.y;

        transform.position = p;
    }
}
