using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fx2 : MonoBehaviour {

	public ParticleSystem ps;
	GameObject player;
    public float effect_timer;
    float timer;

	// Use this for initialization
	void Start () {
		//ps = GetComponent<ParticleSystem> ();
	//	ps.Stop ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}

    // Update is called once per frame
    void Update()
    {

        if (player.GetComponent<PlayerMovement>().hasDoubleJumped && timer < effect_timer)
        {
            //Debug.Log("Working");
            ps.Play();
            timer += Time.deltaTime;
        }
        else if (timer > effect_timer)
        {
            ps.Stop();
            effect_timer = 0;
        }
        else
        {
            ps.Stop();
        }
    }
}

