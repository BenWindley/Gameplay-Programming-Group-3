using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorShut : MonoBehaviour
{

    public GameObject door;
    public GameObject close_pos;

    Vector3 startDoorPos;

    bool close = false;
    GameObject player;

    // Use this for initialization
    void Start()
    {

        startDoorPos = door.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");


    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerMovement>().current_state == PlayerMovement.state.DEAD)
        {
            close = false;
        }

        if (close)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, close_pos.transform.position, Time.deltaTime * 5);
        }
        else
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, startDoorPos, Time.deltaTime * 5);
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == GameObject.FindGameObjectWithTag("Player"))
        {
            close = true;
        }
    }
}
