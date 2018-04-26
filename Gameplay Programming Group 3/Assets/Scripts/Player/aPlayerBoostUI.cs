using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aPlayerBoostUI : MonoBehaviour {

	public Slider boostBar;
	GameObject player;
	Color barcolor;
	public Image fill;
	Vector3 startPos;

	// Use this for initialization
	void Start () {

		startPos = boostBar.transform.position;
		player = GameObject.FindGameObjectWithTag("Player");
		boostBar.value = 5;

		barcolor = fill.color;

	}
	
	// Update is called once per frame
	void Update () {


		if (player.GetComponent<PlayerMovement> ().isBoostUnlocked () == true)
		{
			boostBar.transform.position = startPos;

			if (player.GetComponent<PlayerMovement> ().speed_boost <= 0)
			{
				//boostBar.value = 5;
				boostBar.value = 0 + 5 - player.GetComponent<PlayerMovement> ().boost_cool_time;
				fill.color= Color.gray;

				if (player.GetComponent<PlayerMovement> ().boost_cool_time == 0)
				{
					fill.color = barcolor;
				}

			}
			else
			{
				boostBar.value =  player.GetComponent<PlayerMovement> ().speed_boost;
				fill.color = barcolor;
			}

		} 
		else 
		{
			boostBar.transform.position = new Vector3 (-1000, 0, 0);
		}
	}
}
