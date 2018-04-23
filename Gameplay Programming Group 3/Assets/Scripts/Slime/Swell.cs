using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swell : MonoBehaviour
{
    private float offset = 0.0f;
    private float speed = 1.0f;
    private float amplitude = 1.0f;

    public bool active = true;
    private Vector3 initial;

    void Start()
    {
        offset = Random.Range(-300, 300);
        speed = Random.Range(1.0f, 2.0f);
        amplitude = Random.Range(0.05f, 0.1f);

        initial = transform.localScale;
    }

    // Update is called once per frame
    void Update ()
    {
        transform.localScale = initial * ( 1 + amplitude / 2 + (amplitude / 2) * Mathf.Sin(speed * Time.fixedTime + offset));
    }
}
