using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    private float offset = 0.0f;
    private float speed = 1.0f;
    private float amplitude = 1.0f;

    public bool active = true;
    private Vector3 start_pos;

	void Start ()
    {
        offset = Random.Range(-300, 300);
        speed = Random.Range(1.0f, 2.0f);
        amplitude = Random.Range(-0.1f, 0.1f);

        start_pos = transform.localPosition;
    }
	
	void Update ()
    {
        if(active)
            transform.localPosition = new Vector3(start_pos.x, start_pos.y + amplitude * Mathf.Sin(speed * Time.fixedTime + offset), start_pos.z);
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, start_pos, 0.01f);
        }
	}
}
