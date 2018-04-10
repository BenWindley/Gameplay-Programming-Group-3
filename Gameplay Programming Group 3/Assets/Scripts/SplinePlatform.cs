using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePlatform : MonoBehaviour
{
    public BezierSpline spline;
    public GameObject platform;

    [Range(0.1f, 2.0f)]
    public float platform_speed = 1.0f;

    [Range(0.0f, 5.0f)]
    public float platform_delay = 1.0f;

    [Range(1, 50)]
    public int smoothness = 20;

    [Range(0, 2)]
    public float inertia_multiplier = 1.0f;

    public bool player_activated = true;

    public bool tilting_platform = false;

    public bool loop = false;

    public List<Vector3> points;

    public int current_point = 0;
    private int target_point = 0;
    private bool move_back = true;
    private Vector3 change;
    private GameObject player;

    public bool player_on_platform = false;
    private Vector3 initial_platform_rotation;

    private void Awake()
    {
        Vector3 point = spline.GetPoint(0f);

        int steps = smoothness * spline.CurveCount;

        for (int i = 0; i < steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            points.Add(point);
        }

        point = spline.GetPoint(steps + 1);
        points.Add(point);

        platform.transform.position = points[0];

        player = GameObject.FindGameObjectWithTag("Player");
        initial_platform_rotation = platform.transform.rotation.eulerAngles;
    }

    private void ChangeDirection()
    {
        if (player_activated)
        {
            if (player_on_platform)
            {
                move_back = !move_back;
                CancelInvoke("ChangeDirection");
            }
            else
            {
                move_back = true;
                CancelInvoke("ChangeDirection");
            }
        }
        else
        {
            move_back = !move_back;
            CancelInvoke("ChangeDirection");
        }
    }

    public void LaunchPlayer()
    {
        change = new Vector3(change.x, 0, change.z);
        player.GetComponent<PlayerMovement>().air_launch = change * inertia_multiplier;
    }

    public void MovePlatform()
    {
        if (!(target_point == -1 || target_point == points.Count))
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, points[target_point], platform_speed);

        if (platform.transform.position == points[target_point])
        {
            if (move_back && target_point > 0)
                target_point--;
            else if (!move_back && target_point < points.Count - 1)
                target_point++;
            else
                Invoke("ChangeDirection", platform_delay);
        }
        else
            CancelInvoke("ChangeDirection");
    }

    private void ResetPlatform()
    {
        current_point = 0;
        target_point = 0;

        platform.transform.position = points[0];
    }

    public void RotatePlatform()
    {
        if (player_on_platform)
        {
            Vector3 diff = 20 * (platform.transform.position - player.transform.position);

            Vector3 r = initial_platform_rotation + new Vector3(Mathf.Clamp(diff.z, -20, 20),
                                                                            0,
                                                                            -Mathf.Clamp(diff.x, -20, 20));

            platform.transform.rotation = Quaternion.Euler (Vector3.Lerp(initial_platform_rotation, r, 0.1f));
        }
        else
        {
            platform.transform.rotation = Quaternion.Euler(initial_platform_rotation);
        }
    }

    private void FixedUpdate()
    {
        change = platform.transform.position;

        if (player_activated)
        {
            if (player_on_platform)
            {
                MovePlatform();
            }
            else
            {
                MovePlatform();
            }
        }
        else
        {
            MovePlatform();
        }

        if (tilting_platform)
            RotatePlatform();

        change -= platform.transform.position;
        change *= -1;

        if(player_on_platform)
            player.transform.position += change;
    }
}
