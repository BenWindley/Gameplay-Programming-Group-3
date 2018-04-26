using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePlatform : MonoBehaviour
{
    public GameObject platform;

    [Range(0, 2)]
    public float inertia_multiplier = 1.0f;

    private bool was_player_activated = false;
    public bool player_activated = true;
    public bool moves_player = true;

    private Vector3 start_pos;
    private Vector3 change;
    private Vector3 previous_pos;
    private GameObject player;

    public bool player_on_platform = false;
    private Vector3 initial_platform_rotation;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        previous_pos = transform.position;
        start_pos = transform.position;
        was_player_activated = GetComponent<SplineWalker>().step_on_to_start;
    }
    
    public void LaunchPlayer()
    {
        change = new Vector3(change.x, change.y * 0.5f, change.z);
        player.GetComponent<PlayerMovement>().air_launch = change * inertia_multiplier * 10;
    }

    private void Update()
    {
        change = platform.transform.position - previous_pos;

        if (player_on_platform && moves_player)
        {
            player.transform.position += change;
        }

        previous_pos = transform.position;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (was_player_activated)
        {
            if (col.gameObject == player)
            {
                GetComponent<SplineWalker>().step_on_to_start = false;
            }
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (was_player_activated)
        {
            if (col.gameObject == player)
            {
                GetComponent<SplineWalker>().step_on_to_start = true;
            }
        }
    }
}
