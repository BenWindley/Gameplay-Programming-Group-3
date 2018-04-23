using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    public Transform targetPos;
    public GameObject redSwitch;
    public GameObject door;
    public GameObject player;

    bool doorClosed;

    void Start()
    {
        doorClosed = false;
        camera1.GetComponent<Camera>().enabled = true;
        camera2.GetComponent<Camera>().enabled = false;
    }

    void Update()
    {
        if (doorClosed == true)
        {
            openDoor();
        }
    }

	void OnTriggerStay ()
    {
		if (Input.GetKeyDown(KeyCode.E) && (player.transform.rotation.eulerAngles.y > 50f) && (player.transform.rotation.eulerAngles.y < 120f) && (doorClosed == false)) //player facing switch
        {
            camera1.GetComponent<Camera>().enabled = false;
            camera2.GetComponent<Camera>().enabled = true;

            //player.GetComponent<RPGCharacterControllerFREE>().enabled = false;
            player.transform.rotation = targetPos.transform.rotation;

            GameObject.FindGameObjectWithTag("Player").transform.position = targetPos.transform.position; // moves player to ideal position
            //GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterControllerFREE>().Attack(1); // makes player attack

            camera2.GetComponent<Animator>().SetBool("PlayAnimation", true);
            CancelInvoke("SwitchBack");
            Invoke("SwitchBack", 2.5f);
            Invoke("pushButton", 0.45f);
        }
	}

    void SwitchBack()
    {
        camera1.GetComponent<Camera>().enabled = true;
        camera2.GetComponent<Camera>().enabled = false;
    }

    void pushButton()
    {
        redSwitch.transform.localPosition = new Vector3 (0, 0, - 0.25f);
        doorClosed = true;

    }

    void openDoor()
    {
        if (door.transform.position.y >= -1.75f)
        {
            door.transform.Translate(Vector3.down * Time.deltaTime);
        }
        else
        {
            door.transform.position = new Vector3(door.transform.position.x, -1.75f, door.transform.position.z);
        }
    }

}
