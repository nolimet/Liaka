using UnityEngine;
using System.Collections;

public class TestCoin : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (GameManager.instance)
            Debug.Log("THIS IS A TEST SCRIPT") ;
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.B))
        {
            PickupBase p;
            for (int i = 0; i < 4; i++)
            {
                p = PickupPool.GetObject(PickupBase.PickupType.Coin);
                p.SetMovementType(PickupBase.Movement.Dynamic);
                p.setVelocity(new Vector2(Random.Range(-2f, 2f), 3));
                p.setPosition(transform.position);
                p.startBehaviours();
            }
        }
	}
}
