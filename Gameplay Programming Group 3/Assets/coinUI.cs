using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class coinUI : MonoBehaviour
{
    PlayerMovement player_script;
    public Text countText;

    // Use this for initialization
    void Start ()
    {
        player_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        countText.text = player_script.getCoinCount().ToString();
    }
}
