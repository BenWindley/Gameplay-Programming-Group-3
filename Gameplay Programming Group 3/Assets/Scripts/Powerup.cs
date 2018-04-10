using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum powerup
    {
        DOUBLEJUMP,
        SPEEDBOOST
    }

    public powerup currentPowerup;

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            bool will_destroy = false;

            if (currentPowerup == powerup.DOUBLEJUMP)
            {
                if (col.GetComponent<PlayerMovement>().DoubleJump())
                {
                    will_destroy = true;
                }
            }

            if (currentPowerup == powerup.SPEEDBOOST)
            {
                col.GetComponent<PlayerMovement>().SpeedBoost();

                will_destroy = true;
            }

            if(will_destroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
