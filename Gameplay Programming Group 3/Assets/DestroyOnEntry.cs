using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEntry : MonoBehaviour
{
    public GameObject destroy_this_object;
    public List<GameObject> move_objects;
    public float target_wall_height = 5.0f;

    private bool entered = false;

	void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            entered = true;
            Destroy(destroy_this_object);
        }
    }

    void Update()
    {
        if (entered)
        {
            foreach (GameObject wall in move_objects)
            {
                if (wall.transform.position.y != target_wall_height)
                {
                    wall.transform.position = Vector3.MoveTowards(wall.transform.position, new Vector3(wall.transform.position.x, target_wall_height, wall.transform.position.z), 1);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
