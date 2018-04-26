using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody rb;
    public float fallDelay;
	void Start ()
    {
        rb = GetComponent<Rigidbody>();	
	}

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.isKinematic = false;

        yield return 0;
    }
	

}
