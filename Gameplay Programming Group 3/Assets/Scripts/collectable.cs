using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : MonoBehaviour {

	public int collectibleType;

	//public GameObject idleFx;
	public GameObject explosionFx;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player")
		{
			//GameObject.FindGameObjectWithTag ("Manager").GetComponent<collectibleManager> ().AddCollected (collectibleType);

			switch (collectibleType)
			{
			case 0:
				GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ().speed_boost = 10;
				break;

			case 1:
				GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ().DoubleJump();
				break;

			default:
				break;
			}

			if (explosionFx)
			{
				Instantiate (explosionFx, transform.position, transform.rotation);
			}

            Debug.Log("POWERUPPED");
			Destroy (this.gameObject);
		}
	}
}
