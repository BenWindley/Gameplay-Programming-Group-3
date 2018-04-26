using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostSpawn : MonoBehaviour
{
    public GameObject to_spawn;
    public GameObject move_to_pos;

    private bool cam_once = false;

    bool has_Spawned = false;

    void Update()
    {
        if (!has_Spawned)
        {
            foreach (GameObject slime in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (slime.GetComponent<SlimeBehaviour>() != null ||
                    slime.GetComponent<Enemy>() != null)
                {
                    return;
                }
            }

            to_spawn = Instantiate(to_spawn);
            to_spawn.transform.position = transform.position;

            if (!cam_once)
            {
                Camera.main.transform.parent.transform.parent.GetComponent<CameraManager>().ScenicMode(2.5f, to_spawn.transform.position);
                cam_once = true;
            }

            has_Spawned = true;
        }
        else
        {
            to_spawn.transform.position = Vector3.MoveTowards(to_spawn.transform.position, move_to_pos.transform.position, Time.deltaTime * 5);
        }
    }
}
