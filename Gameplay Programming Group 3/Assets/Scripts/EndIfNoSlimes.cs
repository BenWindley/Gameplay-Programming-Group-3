using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndIfNoSlimes : MonoBehaviour
{
    void ChangeScene()
    {
        SceneManager.LoadScene("HubScene");
    }

    void Update()
    {
		foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(Vector3.Distance(transform.position, enemy.transform.position) < 30)
            {
                CancelInvoke("ChangeScene");
                return;
            }
        }

        Invoke("ChangeScene", 3.0f);
	}
}
