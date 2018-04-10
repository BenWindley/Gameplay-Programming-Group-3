using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public List<GameObject> enemies;

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Enemy")
        {
            foreach(GameObject enemy in enemies)
            {
                if(col.gameObject == enemy)
                {
                    return;
                }
            }
            enemies.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            enemies.Remove(col.gameObject);
        }
    }

    private void Update()
    {
        if (enemies.Count > 0)
        {
            if (enemies[0] == null)
            {
                enemies.Remove(enemies[0]);
            }
        }
    }
}
