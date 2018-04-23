using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public GameObject door;
    public Transform desired_door_opening_posiion;
    private Vector3 button_end;
    private GameObject player;

    private bool activated = false;
    private Transform button_end_position;

    public Camera cinematic_camera;
    private Camera main_camera;

	void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        main_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        button_end = transform.position - new Vector3(0, 0, -0.1f);
    }

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player")
        {
            float player_rotation = player.transform.rotation.eulerAngles.y;
            
            if ((player.GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("Idle -> Attack") ||
                (player.GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("Locomotion -> Attack"))) &&
               !activated &&
               ((player_rotation >= 0 &&
                player_rotation <= 45) ||
               (player_rotation <= 360 &&
                player_rotation >= 315)))
            {
                //Interacting with door
                SetupCinematicCamera();
            }
        }
    }

    void SetupCinematicCamera()
    {
        if (!IsInvoking("OpenDoor") && !IsInvoking("SwitchCameraBack"))
        {
            Invoke("OpenDoor", 0.3f * (player.GetComponent<PlayerMovement>().speed_boost > 0 ? 1.5f : 1.0f));            
        }
        else
        {
            cinematic_camera.enabled = true;
            main_camera.enabled = false;
        }

        player.transform.position = desired_door_opening_posiion.position;
        player.transform.rotation = desired_door_opening_posiion.rotation;
    }

    void OpenDoor()
    {
        activated = true;

        Invoke("SwitchCameraBack", 1.2f * (player.GetComponent<PlayerMovement>().speed_boost > 0 ? 1.5f : 1.0f));
    }

    void SwitchCameraBack()
    {
        cinematic_camera.enabled = false;
        main_camera.enabled = true;
    }

    void FixedUpdate()
    {
        if(activated)
        {
            transform.position = Vector3.MoveTowards(transform.position, button_end, 0.02f);

            if(transform.position.z == button_end.z)
            {
                transform.position = button_end;
                door.GetComponent<Door>().open = true;
            }
        }
    }
}