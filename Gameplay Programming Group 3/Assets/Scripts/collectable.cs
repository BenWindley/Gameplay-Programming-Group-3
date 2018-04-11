using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : MonoBehaviour {

	[System.Serializable]
	public enum collectType
	{
		speed_boost,
		double_jump
	};

	public collectType pickup;

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

			switch (pickup)
			{
			case collectType.speed_boost:
				GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ().speed_boost = 10;
				break;

			case collectType.double_jump:
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
