using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float min_height;
	
	void Update ()
    {
		if (transform.position.y < min_height)
        {
            Destroy(gameObject);
        }
	}
}
