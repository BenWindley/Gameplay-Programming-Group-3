using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubworldScenic : MonoBehaviour {

	public GameObject look_at;

	bool cam_once = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col)
	{
		//if (!cam_once)
		//{
		if(col.gameObject == GameObject.FindGameObjectWithTag("Player"))
		{
			if (!cam_once) {
				Camera.main.transform.parent.transform.parent.GetComponent<CameraManager> ().ScenicMode (2.5f, look_at.transform.position);
				cam_once = true;
			}
		}
	}
}
