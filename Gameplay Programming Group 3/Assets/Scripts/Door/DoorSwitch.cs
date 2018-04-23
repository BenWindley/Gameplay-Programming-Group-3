using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public GameObject door;
    public Transform desired_door_opening_posiion;
    private GameObject player;
    public Animator camera_anim;

    public bool activated = false;
    private Transform button_end_position;

    public Camera cinematic_camera;
    private Camera main_camera;

	void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        main_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cinematic_camera.enabled = false;
    }

    void OnTriggerStay(Collider col)
    {
        Debug.Log("Player in Collider");
        if(col.tag == "Player")
        {
            float player_rotation = player.transform.rotation.eulerAngles.y;
            
            if ((player.GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("Idle -> Attack") ||
                (player.GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("Locomotion -> Attack"))) &&
               !activated/* &&
               ((player_rotation >= 0 + transform.eulerAngles.y &&
                player_rotation <= 45 + transform.eulerAngles.y) ||
               (player_rotation <= 360 + transform.eulerAngles.y &&
                player_rotation >= 315 + transform.eulerAngles.y))*/)
            {
                //Interacting with door
                SetupCinematicCamera();
                camera_anim.SetTrigger("Play");
            }
        }
    }

    void SetupCinematicCamera() // switches cameras
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
        door.GetComponent<Door>().open = true;
        Invoke("SwitchCameraBack", 2.5f * (player.GetComponent<PlayerMovement>().speed_boost > 0 ? 1.5f : 1.0f));
    }

    void SwitchCameraBack()
    {
        cinematic_camera.enabled = false;
        main_camera.enabled = true;
        camera_anim.SetTrigger("Stop");
    }
}