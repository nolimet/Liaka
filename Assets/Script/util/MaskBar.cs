using UnityEngine;
using System.Collections;

public class MaskBar : MonoBehaviour {

    public enum scaleDirection
    {
        Vertical,
        Horizontal
    }
    public scaleDirection direction;

    Vector2 startSize;
    RectTransform rect;
    void Awake()
    {
        rect = (RectTransform)transform;

        startSize = rect.sizeDelta;
    }

    /// <summary>
    /// Sets value
    /// </summary>
    /// <param name="f">value that the bar wil represent. Value between 0 and 1</param>
	public void setValue(float f)
    {
        if (f > 1)
            f = 1;
        if (f < 0)
            f = 0;


        switch (direction)
        {
            case scaleDirection.Horizontal:
                rect.sizeDelta = new Vector2(startSize.x * f, startSize.y);
                break;

            case scaleDirection.Vertical:
                rect.sizeDelta = new Vector2(startSize.x, startSize.y * f);
                break;
        }
    }
}
