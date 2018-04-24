using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDie : MonoBehaviour
{
    private Vector3 start_position;

    [Range(-100f, 0f)]
    public float min_height = -5.0f;

    void Start ()
    {
        start_position = transform.position;

    }

	void Update ()
    {
		if(transform.position.y < min_height)
        {
            transform.position = start_position;

            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
	}
}
