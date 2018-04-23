using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrolPath : MonoBehaviour {

	public GameObject[] path;

	// Use this for initialization
	void Awake () {

		Transform[] children = GetComponentsInChildren<Transform> ();

		path = new GameObject[children.Length - 1];

		for(int i = 0; i < children.Length - 1; i++)
		{
			path [i] = children [i + 1].gameObject;
		}
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	void LateUpdate()
	{
		for (int i = 0; i < path.Length - 1; i++) 
		{
			Debug.DrawLine (path [i].transform.position, path [i+1].transform.position, Color.green);
		}

		Debug.DrawLine (path [path.Length - 1].transform.position, path [0].transform.position, Color.green);
	}
}
