using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyEffect : MonoBehaviour {

	float timer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;

		if (timer >= GetComponent<ParticleSystem> ().main.duration) 
		{
			Destroy (gameObject);
		}
		
	}
}
