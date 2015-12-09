using UnityEngine;
using System.Collections;

public class AntiShaker : MonoBehaviour
{

    protected Vector3 StartPos;

    void Awake()
    {
        StartPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (!ScreenShaker.Shaking)
            transform.localPosition = StartPos;
        else
            transform.localPosition = StartPos + (Vector3)ScreenShaker.currenOffSet;
    }
}
