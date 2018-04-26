using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fx2 : MonoBehaviour {

	public ParticleSystem ps;
	GameObject player;
    public float effect_timer;
    float timer;
	bool go = false;

	// Use this for initialization
	void Start () {
		//ps = GetComponent<ParticleSystem> ();
	//	ps.Stop ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}

    // Update is called once per frame
    void Update()
    {

		if (player.GetComponent<PlayerMovement>().hasDoubleJumped) 
		{
			//Debug.Log("Working");
			ps.Play ();
		}
		else
		{
			ps.Stop ();
		}
			
    }
}

