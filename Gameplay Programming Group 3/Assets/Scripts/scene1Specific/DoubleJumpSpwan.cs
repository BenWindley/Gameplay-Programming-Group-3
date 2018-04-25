using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpSpwan : MonoBehaviour {

    public GameObject to_spawn;
    public GameObject move_to_pos;

    bool has_Spawned = false;

    public GameObject door_unlock;
    public GameObject door_move;

    bool cam_once = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        if (has_Spawned == false)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                to_spawn = Instantiate(to_spawn);
                to_spawn.transform.position = transform.position;
                has_Spawned = true;
            }
        }
        else
        {
            to_spawn.transform.position = Vector3.MoveTowards(to_spawn.transform.position, move_to_pos.transform.position, Time.deltaTime * 5);
            door_unlock.transform.position = Vector3.MoveTowards(door_unlock.transform.position, door_move.transform.position, Time.deltaTime * 5);

            if (!cam_once)
            {
                Camera.main.transform.parent.transform.parent.GetComponent<CameraManager>().ScenicMode(2.5f, door_unlock.transform.position);
                cam_once = true;
            }
        }
    
		
	}
}
