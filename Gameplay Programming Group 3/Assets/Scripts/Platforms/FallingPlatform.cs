using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody rb;
    public float fallDelay;
    public float respawnTime;

    private Vector3 start_pos;
    private Quaternion start_rotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        start_pos = transform.position;
        start_rotation = transform.rotation;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);

        rb.isKinematic = false;

        StartCoroutine(Respawn());

        yield return 0;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        rb.isKinematic = true;
        transform.position = start_pos;
        transform.rotation = start_rotation;

        yield return 0;
    }
}
