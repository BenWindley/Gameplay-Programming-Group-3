using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : MonoBehaviour {

	[System.Serializable]
	public enum collectType
	{
		speed_boost,
		double_jump,
        health
	};

	public collectType pickup;

	//public GameObject idleFx;
	public GameObject explosionFx;

    public float health_increase;

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
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            switch (pickup)
			{
			case collectType.speed_boost:
                    //	GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ().speed_boost = 10;
                    p.GetComponent<PlayerMovement>().unlockBoost();
                break;

			case collectType.double_jump:
                    //	GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ().DoubleJump();
                    p.GetComponent<PlayerMovement>().unlockDoubleJump();
                break;

                case collectType.health:
                    if (p.GetComponent<PlayerMovement>().health < p.GetComponent<PlayerMovement>().max_health)
                    {
                        p.GetComponent<PlayerMovement>().health += health_increase;
                    }
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
