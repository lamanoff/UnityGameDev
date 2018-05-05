using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    private GameObject mainCamera;

    void Start()
        => mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

    void Update()
        => transform.LookAt(mainCamera.transform);
}