using UnityEngine;

public class SplineWalker : MonoBehaviour
{
	public BezierSpline spline;

	public float duration;

	public bool lookForward;

	public SplineWalkerMode mode;

	public float progress;
	private bool goingForward = true;

    public bool step_on_to_start = false;

    private bool initial_step = false;

    private float time_till_change_direction = 0.0f;
    private float max_time_till_change_direction = 2.0f;

    void Start()
    {
        transform.localPosition = spline.GetPoint(progress);

        initial_step = step_on_to_start;
    }
    private void Update()
    {
        if (!step_on_to_start)
        {
            time_till_change_direction = 0.0f;

            if (goingForward)
            {
                progress += Time.deltaTime / duration;

                if (progress > 1f)
                {
                    if (mode == SplineWalkerMode.Once)
                    {
                        progress = 1f;
                    }
                    else if (mode == SplineWalkerMode.Loop)
                    {
                        progress -= 1f;
                    }
                    else
                    {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            }
            else
            {
                progress -= Time.deltaTime / duration;

                if (progress < 0f)
                {
                    progress = -progress;
                    goingForward = true;
                }
            }

            Vector3 position = spline.GetPoint(progress);
            transform.localPosition = position;

            if (lookForward)
            {
                transform.LookAt(position + spline.GetDirection(progress));
            }
        }
        else
        {
            if (time_till_change_direction > max_time_till_change_direction)
            {
                progress = progress < 0 ? 0 : progress - Time.deltaTime / duration;

                Vector3 position = spline.GetPoint(progress);
                transform.localPosition = position;
            }
            else
            {
                time_till_change_direction += Time.deltaTime;
            }
        }
    }
}