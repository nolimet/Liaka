using UnityEngine;
using System.Collections;

public class MoveBoxScaler : MonoBehaviour {

    public bool useRectransMode = false;

    [Header("Default Scaling mode")]
    [SerializeField]
    new Camera camera;

    public Vector3 screenSize = Vector3.zero;

    [Tooltip("Sprite's Size in pixel used to make sure it's scaled correctly")]
    public Vector2 StartSize = Vector2.one;
    [Tooltip("exstra size that will be added after properscaling")]
    public Vector2 ExstraSizeMult = Vector2.one;

    [SerializeField, Tooltip("Should object Scale in this direction?")]
    bool Vertical = false, Horizontal = false;

    

    void Awake()
    {
        if (useRectransMode)
            rectTransMode();
        else
            defaultMode();
    }

    void defaultMode()
    {
if (camera == null)
            camera = Camera.main;

        StartSize /= 100;

        Vector3 p1 = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        p1.x /= StartSize.x;
        p1.y /= StartSize.y;

        p1 *= 2f;
        if (!Vertical)
            p1.y = transform.localScale.y;
        if (!Horizontal)
            p1.x = transform.localScale.x;

        p1.Scale(ExstraSizeMult);
        transform.localScale = new Vector3(p1.x, p1.y, 1); //* 2f;
        screenSize = new Vector3(p1.x, p1.y, 1); //* 2f;
    }

    void rectTransMode()
    {
        RectTransform r = (RectTransform)transform;
        Vector2 n;
        Vector2 Aspect = new Vector2(Screen.width, Screen.height);

        if (r.sizeDelta.x > r.sizeDelta.y)
            n = new Vector2(r.sizeDelta.x, r.sizeDelta.x);
        else
            n = new Vector2(r.sizeDelta.y, r.sizeDelta.y);

        //r.sizeDelta = n;
    }
}
