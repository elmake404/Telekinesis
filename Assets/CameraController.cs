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
    }

    public Ray GetRayFromScreen(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        return ray;
    }

    
}
