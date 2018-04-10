using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fx : MonoBehaviour {

	public ParticleSystem ps;
	GameObject player;

	// Use this for initialization
	void Start () {
		//ps = GetComponent<ParticleSystem> ();
		ps.Stop ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {

		if (player.GetComponent<PlayerMovement>().speed_boost > 0) 
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
