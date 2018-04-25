using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject obj;
    public float spawn_delay;
    private float timer;
    private Vector3 spawn_point;

    void Start()
    {
        spawn_point = transform.position;
        timer = spawn_delay;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Instantiate(obj, spawn_point, transform.rotation);
            timer = spawn_delay;
        } 
    }
}
