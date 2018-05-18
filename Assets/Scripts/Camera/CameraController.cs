using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject Target;
    public float Smoothing = 5f;
    public float Distance = 10f;

    Vector3 offset;

    private void FindTarget()
    {
        Target = GameObject.FindGameObjectWithTag("Player");
        if (Target == null)
            return;
        offset = transform.position - Target.transform.position;
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
        var targetCamPos = Target.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Smoothing * Time.deltaTime);
    }
}