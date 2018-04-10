using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPlayerDetector : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            GetComponentInParent<SplinePlatform>().player_on_platform = true;
            
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            GetComponentInParent<SplinePlatform>().player_on_platform = false;
            GetComponentInParent<SplinePlatform>().LaunchPlayer();
        }
    }
}
