using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEntry : MonoBehaviour
{
    public GameObject destroy_this_object;

	void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Destroy(destroy_this_object);
            Destroy(gameObject);
        }
    }
}
