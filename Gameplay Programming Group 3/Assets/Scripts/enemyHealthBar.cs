using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealthBar : MonoBehaviour {

	public Slider bar;

	public float health;
	//public float startX;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () 
	{
		bar.value = health;
	}

	public void setMaxValue(int m)
	{
		bar.maxValue = m;
	}
}
