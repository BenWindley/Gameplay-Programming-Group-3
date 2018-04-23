using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineLerp : MonoBehaviour
{
	public BezierSpline spline;

    public List<Vector3> points;

    public int current_point = 0;

	private void Awake ()
    {
        Vector3 point = spline.GetPoint(0f);

        int steps = 200 * spline.CurveCount;

        for (int i = 0; i < steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            points.Add(point);
        }
    }

    public Vector3 GetClosestToPoint(Vector3 point)
    {
        Vector3 current_closest_point = points[0];

        for (int i = 0; i < points.Count; i++)
        {

            Vector2 spline_point = new Vector2(points[i].x, points[i].z);            
            Vector2 player_point = new Vector2(point.x, point.z);
            Vector2 current = new Vector2(points[current_point].x, points[current_point].z);
            
            if (Vector2.Distance(spline_point, player_point) < Vector2.Distance(current, player_point))
            {
                current_point = i;
            }
        }
        
        return points[current_point];
    }

    public float GetYRotation()
    {
        float d_x = points[current_point + 1].x - points[current_point].x;
        float d_y = points[current_point + 1].z - points[current_point].z;

        float angle = 0;

        if(d_x > 0)
        {
            if(d_y > 0)
            {
                angle = - 90 + Mathf.Rad2Deg * Mathf.Atan2(d_x, d_y);
            }
            else
            {
                angle = 90 - Mathf.Rad2Deg * Mathf.Atan2(d_x, d_y);
            }
        }
        else
        {
            if (d_y > 0)
            {
                angle = Mathf.Rad2Deg * Mathf.Atan2(d_x, d_y) - 90;
            }
            else
            {
                return 90 - Mathf.Rad2Deg * Mathf.Atan2(d_x, d_y);
            }
        }

        return angle;
    }
}