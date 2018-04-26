using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool open = false;
    private float open_rotation = 90;
    private float close_rotation = 0;
	
    void Start()
    {
        open_rotation += transform.rotation.eulerAngles.y;
        close_rotation += transform.rotation.eulerAngles.y;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (open)
        {
            if (transform.rotation.y < open_rotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, open_rotation, 0)), 0.1f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, open_rotation, 0));
            }
        }
        else
        {
            if (transform.rotation.y > close_rotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, close_rotation, 0)), 0.1f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, close_rotation, 0));
            }
        }
	}
}
