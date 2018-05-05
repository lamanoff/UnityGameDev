using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public float smoothing = 5f;

    Vector3 offset;

    private void FindTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }
        var targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}