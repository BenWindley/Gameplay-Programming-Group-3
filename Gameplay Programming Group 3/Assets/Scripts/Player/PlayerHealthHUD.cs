using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealthHUD : MonoBehaviour
{
    private float current = 0.0f;
    private float target = 0.0f;
    private PlayerMovement player;
    public CanvasGroup canvas;
    public Image overlay;

    private float previous_health;
    private float time = 0.0f;
    
    // Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        current = 0;
        target = player.health / player.max_health * 100;
        overlay.color = new Color(1, 0, 0, 1 - (target / 100));

        previous_health = player.health;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Time.fixedTime > .5f)
        {
            target = player.health / player.max_health * 100;
            current = Mathf.MoveTowards(current, target, 2f);

            GetComponent<Slider>().value = current;

            if (target <= 0.0f)
            {
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0.0f, 0.1f);
            }
        }

        GetComponent<Slider>().value = current;

        {
            if (previous_health > player.health)
            {
                time = 0.2f;
            }

            previous_health = player.health;
        }

        if(time > 0)
        {
            overlay.color = new Color(1 - player.health / player.max_health, 0, 0, Mathf.Lerp(overlay.color.a, 0.7f, 0.1f));
            time -= Time.deltaTime;
        }
        else if (player.health > 0f)
        {
            overlay.color = new Color(1 - player.health / player.max_health, 0, 0, Mathf.Lerp(overlay.color.a, 0.5f - player.health / player.max_health, 0.1f));
        }
    }
}
