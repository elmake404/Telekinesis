using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [HideInInspector] public Camera mainCamera;


    private void OnEnable()
    {
        instance = this;

    }

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        //Debug.Log((1.0f + 0.5625f) - mainCamera.aspect);
        mainCamera.fieldOfView *= ((1.0f + 0.5625f) - mainCamera.aspect);
        
    }

    public Ray GetRayFromScreen(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        return ray;
    }

    private void OnDisable()
    {
        instance = null;
        //Debug.Log("Destroy");
    }
}
