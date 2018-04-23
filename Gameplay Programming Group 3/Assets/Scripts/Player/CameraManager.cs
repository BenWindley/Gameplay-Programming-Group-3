using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum state
    {
        OUTSIDE,
        INSIDE,
        SCENIC,
        SPLINE,
        COMBAT
    }

    public state camera_state = state.OUTSIDE;

    public float lerp_speed = 0.1f;

    public Transform outside_pos;
    public Transform look_at_outside;
    public Transform inside_pos;
    public Transform look_at_inside;
    public Transform scenic_pos;
    private Vector3 look_at_scenic;
    public Transform look_at_target;

    public Transform combat_pos;
    private Vector3 default_combat_pos;
    private Vector3 combat_target;
    private float combat_camera_distance;

    private List<Vector3> combat_targets = new List<Vector3>();
    
    public float spline_rotation;

    private float scenic_timer = 0.0f;
    public float rotation_speed = 1.0f;
    public float target_rotation = 0.0f;

    private bool inside = false;
    private bool lock_on_mode = true;

    public GameObject player_camera;
    private GameObject player;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        combat_camera_distance = Vector3.Distance(new Vector3(combat_pos.position.x, 0, combat_pos.position.z), new Vector3(transform.position.x, 0, transform.position.z));
        default_combat_pos = combat_pos.position;
    }

    public bool IsLockedOnto(GameObject enemy)
    {
        return Vector3.Distance(combat_target, enemy.transform.position) < 1.0f;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position;
        
        combat_targets.Clear();

        if (Input.GetButtonDown("LockOn"))
            lock_on_mode = !lock_on_mode;

        
        if (lock_on_mode)
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if ((enemy.GetComponent("SlimeBehaviour") as SlimeBehaviour) != null)
                {
                    if (Vector3.Distance(player.transform.position, enemy.transform.position) < (camera_state == state.COMBAT ? 40 : 20) &&
                   enemy.GetComponent<SlimeBehaviour>().health > 0 &&
                   enemy.GetComponent<SlimeBehaviour>().size != SlimeBehaviour.SlimeType.Smolo &&
                   enemy.GetComponent<SlimeBehaviour>().canSeePlayer)
                    {
                        camera_state = state.COMBAT;
                        combat_targets.Add(enemy.transform.position);
                    }
                }
                if ((enemy.GetComponent("Enemy") as Enemy) != null)
                {
                    if(Vector3.Distance(player.transform.position, enemy.transform.position) < (camera_state == state.COMBAT ? 40 : 20))
                    {
                        camera_state = state.COMBAT;
                        combat_targets.Add(enemy.transform.position);
                    }
                }
            }

            combat_target = Vector3.zero;

            if (combat_targets.Count > 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Lerp(transform.rotation.eulerAngles.y, 0, 0.1f), 0));
                target_rotation = 0.0f;

                combat_target = combat_targets[0];

                foreach (Vector3 enemy in combat_targets)
                {
                    if (Vector3.Distance(player.transform.position, enemy) < Vector3.Distance(player.transform.position, combat_target))
                    {
                        combat_target = enemy;
                    }
                }

                LookAtLerp(combat_target);

                Vector3 new_pos = combat_target - player.transform.position;
                new_pos = new Vector3(-new_pos.x, 0, -new_pos.z).normalized * combat_camera_distance;
                new_pos = new Vector3(new_pos.x, combat_pos.localPosition.y, new_pos.z);

                combat_pos.localPosition = new_pos;

                CamRaycastAdjust();

                return;
            }
            else
            {
                combat_pos.position = default_combat_pos;
                lock_on_mode = false;
            }
        }

        if (lock_on_mode)
        {
            player_camera.transform.GetChild(0).GetComponent<Camera>().fieldOfView = Mathf.Lerp(player_camera.transform.GetChild(0).GetComponent<Camera>().fieldOfView, 40, 0.1f);
        }
        else
        {
            player_camera.transform.GetChild(0).GetComponent<Camera>().fieldOfView = Mathf.Lerp(player_camera.transform.GetChild(0).GetComponent<Camera>().fieldOfView, 60, 0.1f);
        }

        if (!player.GetComponent<PlayerMovement>().in_spline)
        {
            if (Physics.Raycast(player.transform.position, Vector3.up, 6.0f))
            {
                CancelInvoke("Outside");
                inside = true;

                camera_state = state.INSIDE;
            }
            else
            {
                if (!IsInvoking("Outside"))
                    Invoke("Outside", 0.5f);

                if (!inside)
                    camera_state = state.OUTSIDE;
            }

            if (scenic_timer > 0.0f)
            {
                scenic_timer -= Time.deltaTime;

                if (scenic_timer < 0.0f)
                    scenic_timer = 0.0f;

                camera_state = state.SCENIC;
            }
        }
        else
        {
            camera_state = state.SPLINE;
        }

        switch (camera_state)
        {
            case state.OUTSIDE:
            {
                transform.position = player.transform.position;

                RotateCheck();
                LookAtLerp(look_at_outside.position);

                CamRaycastAdjust();
                break;
            }
            case state.INSIDE:
            {
                transform.position = player.transform.position;

                RotateCheck();
                LookAtLerp(look_at_inside.position);

                CamRaycastAdjust();
                break;
            }
            case state.SCENIC:
            {
                transform.position = player.transform.position;

                LookAtLerp(look_at_scenic);

                CamRaycastAdjust();
                break;
            }
            case state.SPLINE:
            {
                transform.position = player.transform.position;
                
                LookAtLerp(look_at_outside.position);
                    
                player_camera.transform.rotation = Quaternion.Euler(player_camera.transform.rotation.eulerAngles.x, spline_rotation, player_camera.transform.rotation.eulerAngles.z);

                break;
            }
        }        
    }

    public void CompleteRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(Vector3.up * angle);
    }

    public void SplineLerp (Vector3 point)
    {
        player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, point, 0.1f);
    }

    public void ScenicMode(float time, Vector3 target)
    {
        scenic_timer = time;
        look_at_scenic = target;
    }

    void LookAtLerp(Vector3 position)
    {
        look_at_target.position = Vector3.Lerp(look_at_target.position, position, lerp_speed);

        player_camera.transform.LookAt(look_at_target.position);
    }

    void Outside()
    {
        inside = false;
    }
    
    void CamRaycastAdjust()
    {
        Vector3 pos = outside_pos.position;

        switch (camera_state)
        {
            case state.OUTSIDE:
                {
                    pos = outside_pos.position;

                    break;
                }
            case state.COMBAT:
                {
                    pos = combat_pos.position;

                    break;
                }
            case state.INSIDE:
                {
                    pos = inside_pos.position;

                    break;
                }
            case state.SCENIC:
                {
                    pos = scenic_pos.position;

                    break;
                }
        }

        Ray ray = new Ray(look_at_target.position, pos - look_at_target.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Vector3.Distance(look_at_target.position, pos), LayerMask.GetMask("Default")))
        {
            player_camera.transform.position = hit.point;
        }
        else
        {
            player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, pos, lerp_speed * 2);
        }

        //Debug.DrawRay(look_at_target.position, pos - look_at_target.position);
    }

    void RotateCheck()
    {
        if (Input.GetButtonDown("RotateClockwise"))
        {
            target_rotation += 90;
        }

        if (Input.GetButtonDown("RotateAntiClockwise"))
        {
            target_rotation -= 90;
        }

        UpdateRotation();
    }

    void UpdateRotation()
    {
        transform.Rotate(Vector3.up, target_rotation * Time.deltaTime * rotation_speed);
        target_rotation = target_rotation * (1 - Time.deltaTime * rotation_speed);
    }
}