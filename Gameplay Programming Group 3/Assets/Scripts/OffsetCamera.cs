using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetCamera : MonoBehaviour
{
    public CameraManager cam_manager;
	
	void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            cam_manager.target_rotation -= 45.0f;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            cam_manager.target_rotation += 45.0f;
        }
    }
}
