using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSwapper : MonoBehaviour {

    Image i;

    public Sprite[] spr;
    int index;
    
    void Start()
    {
        i = GetComponent<Image>();
    }

	public void Swap()
    {
        index++;
        if (index >= spr.Length)
            index = 0;

        i.sprite = spr[index];
    }
}
