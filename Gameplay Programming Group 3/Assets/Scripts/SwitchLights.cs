using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLights : MonoBehaviour
{
    public Light lights;

    private float min = 0.3f;
    private float max = 1;

	void Start ()
    {
        StartCoroutine(Flicker());
	}
	
    IEnumerator Flicker ()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(min, max));
            lights.enabled = !lights.enabled;
        }
    }
}
