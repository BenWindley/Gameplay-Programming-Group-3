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
            Destroy(gameObject);
        }
    }
}
