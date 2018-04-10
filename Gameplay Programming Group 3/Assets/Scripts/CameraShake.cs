using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0f;
    public float shakeAmount = 0.7f;

    public float time = 0;
    public float magnitude = 0;
    public Vector3 start_pos;

    public void Shake(float d, float m)
    {
        Vector3 start_pos = transform.position;
        
        magnitude = m;
        time = d;

        transform.position = start_pos;
    }

    public void Start()
    {
        start_pos = transform.localPosition;
    }

    public void Update()
    {
        if (time > 0)
        {
            Vector3 new_pos = start_pos;

            new_pos += new Vector3(Random.Range(-1, 1) * magnitude, Random.Range(-1, 1) * magnitude, 0);

            transform.localPosition = Vector3.Lerp(transform.localPosition, new_pos, 0.1f);

            time -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, start_pos,0.1f);
        }
    }
}
