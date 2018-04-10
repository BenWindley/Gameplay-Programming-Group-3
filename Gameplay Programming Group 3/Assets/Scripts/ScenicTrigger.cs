using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenicTrigger : MonoBehaviour
{
    public Transform scenic_location;
    public CameraManager camera_manager;

    public bool trigger_once = false;
    [Range (0.1f, 5.0f)]
    public float scenic_time = 1.0f;
    private bool triggered = false;

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            if (trigger_once)
            {
                if (!triggered)
                {
                    triggered = true;

                    col.GetComponent<PlayerMovement>().Pause(scenic_time);

                    camera_manager.ScenicMode(scenic_time, scenic_location.position);
                }
            }
            else
            {
                triggered = true;

                col.GetComponent<PlayerMovement>().Pause(scenic_time);

                camera_manager.ScenicMode(scenic_time, scenic_location.position);
            }
        }
    }
}
