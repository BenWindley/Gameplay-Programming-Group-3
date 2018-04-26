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
		effect_timer = ps.duration;
	}

    // Update is called once per frame
    void Update ()
	{

		if (player.GetComponent<PlayerMovement> ().hasDoubleJumped) 
		{
			//Debug.Log("Working");
			ps.Play ();

			timer += Time.deltaTime;
		} 

		if (timer > effect_timer) 
		{
			ps.Stop ();

			if (!player.GetComponent<PlayerMovement> ().hasDoubleJumped) 
			{
				timer = 0;
			}


		}
			
	}
}

