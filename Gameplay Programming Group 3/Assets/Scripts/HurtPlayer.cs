using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            player.GetComponent<PlayerMovement>().health--;
            //player.GetComponent<PlayerMovement>().AddForce(air_launch * 200);
            Vector3 force = player.transform.position - transform.position + Vector3.up;
            player.GetComponent<PlayerMovement>().air_launch += new Vector3(force.x, force.y, 0);
            Destroy(gameObject);
        }
    }
}
