using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform Target;
    public float Smoothing = 5f;
    public float Distance = 10f;

    Vector3 offset;

    private void FindTarget()
    {
        Target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - Target.position;
        offset.Normalize();
        offset *= Distance;
    }

    void FixedUpdate()
    {
        if (Target == null)
        {
            FindTarget();
            return;
        }
        var targetCamPos = Target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Smoothing * Time.deltaTime);
    }
}