using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTrigger : MonoBehaviour
{
    public SplineLerp spline_lerp;
    public CameraManager camera_parent;

    private GameObject camera_controller;

    public bool smooth_rotate = false;

    public float camera_change = 0.0f;

    private void Start()
    {
        camera_controller = GameObject.FindGameObjectWithTag("CameraController");
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<PlayerMovement>().in_spline = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<PlayerMovement>().in_spline = false;

            camera_controller.GetComponent<CameraManager>().CompleteRotation(camera_change);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<PlayerMovement>().in_spline = true;
            col.GetComponent<PlayerMovement>().spline_rotation = spline_lerp.GetYRotation();

            camera_parent.SplineLerp(spline_lerp.GetClosestToPoint(col.gameObject.transform.position));

            float target_rotation = spline_lerp.GetYRotation();

            if (Input.GetAxis("Horizontal") > 0)
            {
                target_rotation += 20;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                target_rotation -= 20;
            }

            float rotation = smooth_rotate ?
                             Mathf.MoveTowards(camera_controller.GetComponent<CameraManager>().spline_rotation, target_rotation, 0.2f) :
                             spline_lerp.GetYRotation();

            rotation = smooth_rotate ? Mathf.Lerp(rotation, target_rotation, 0.05f) : 0;
            

            camera_controller.GetComponent<CameraManager>().spline_rotation = rotation;
        }
    }
}