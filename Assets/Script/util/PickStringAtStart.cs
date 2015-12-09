using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class PickStringAtStart : MonoBehaviour {

    public string[] Texts;

	// Use this for initialization
	void Start () {
        GetComponent<UnityEngine.UI.Text>().text = Texts[Random.Range(0, Texts.Length)];
    }

}
