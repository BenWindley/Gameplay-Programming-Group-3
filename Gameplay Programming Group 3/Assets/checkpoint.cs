using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour {

	// Use this for initialization
	void Start () {

		this.GetComponent<Renderer> ().material.color = Color.red;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col)
	{

		foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
		{
			checkpoint.GetComponent<Renderer> ().material.color = Color.red;
		}

		this.GetComponent<Renderer> ().material.color = Color.green;

		GameObject.FindGameObjectWithTag ("Player").GetComponent<NoDie> ().start_position = transform.position;
	}
}
