using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmallMove : MonoBehaviour
{
    public GameObject left_target;
    public GameObject right_target;
    public GameObject default_target;

    public GameObject main_camera;

    void LateUpdate ()
    {
        bool hit_left = false;
        bool hit_right = false;

		if(Physics.Raycast(transform.position, right_target.transform.position - transform.position, 2.0f, LayerMask.GetMask("Default")))
        {
            hit_right = true;

            Debug.Log("Hit Right");
        }
        if (Physics.Raycast(transform.position, left_target.transform.position - transform.position, 2.0f, LayerMask.GetMask("Default")))
        {
            hit_left = false;

            Debug.Log("Hit Left");
        }

        if(hit_left)
        {
            transform.position = Vector3.Lerp(main_camera.transform.position, right_target.transform.position, 0.5f);
        }
        else if(hit_right)
        {
            transform.position = Vector3.Lerp(main_camera.transform.position, left_target.transform.position, 0.5f);
        }
        else
        {
            transform.position = Vector3.Lerp(main_camera.transform.position, default_target.transform.position, 0.1f);
        }

        Debug.DrawRay(main_camera.transform.position, right_target.transform.position - transform.position);
        Debug.DrawRay(main_camera.transform.position, left_target.transform.position - transform.position);
    }
}
