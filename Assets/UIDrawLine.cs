using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrawLine : MonoBehaviour
{
    public UIMeshRenderer uIMeshRenderer;
    public float widthLine;
    public float minStepLine;
    private List<Vector2> drawPoints = new List<Vector2>();
    private ConnectedObject[] connectedObjects = new ConnectedObject[2];
    public LayerMask hitToOnject;
    private SlowMotionControl slowMotionControl;


    private void Start()
    {
        slowMotionControl = GeneralManager.instance.slowMotionControl;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (slowMotionControl.isSlowMotion == true) { return; }
            CheckEntryRaycast();
        }

        if (Input.GetMouseButton(0))
        {
            
            AddDrawPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            CheckOutroRaycast();
        }
    }

    private void AddDrawPoint(Vector2 point)
    {
        if (drawPoints.Count == 0)
        {
            return;
        }

        float distance = Vector2.Distance(drawPoints[drawPoints.Count - 1], point);
        if (distance > minStepLine)
        {
            drawPoints.Add(point);
            uIMeshRenderer.meshedPoints.AddPoint(point);
            uIMeshRenderer.UpdateMesh();
            return;
        }
        
    }

    private void CheckEntryRaycast()
    {
        Ray ray = CameraController.instance.GetRayFromScreen(Input.mousePosition);
        RaycastHit raycastHit;
        bool isHit = Physics.Raycast(ray, out raycastHit, 100f, hitToOnject);

        if (isHit == true)
        {
            connectedObjects[0] = new ConnectedObject(raycastHit.point, raycastHit.rigidbody, raycastHit.collider);
            drawPoints.Add(Input.mousePosition);
            uIMeshRenderer.meshedPoints.AddPoint(Input.mousePosition);
            uIMeshRenderer.UpdateMesh();
        }
        
        return;
    }

    private void CheckOutroRaycast()
    {
        Ray ray = CameraController.instance.GetRayFromScreen(Input.mousePosition);
        RaycastHit raycastHit;
        bool isHit = Physics.Raycast(ray, out raycastHit, 100f, hitToOnject);

        if (isHit == true)
        {
            connectedObjects[1] = new ConnectedObject(raycastHit.point, raycastHit.rigidbody, raycastHit.collider);
            uIMeshRenderer.meshedPoints.Clear();
            uIMeshRenderer.UpdateMesh();

            GeneralManager.instance.ropesController.CreateNewRope(GetSimpleSpline(), connectedObjects);
            drawPoints.Clear();
            
        }
        else if (isHit == false)
        {
            uIMeshRenderer.meshedPoints.Clear();
            uIMeshRenderer.UpdateMesh();
            drawPoints.Clear();
        }
    }

    private List <Vector2> GetSimpleSpline()
    {
        List<Vector2> simpledPoints = DuglasKeper.GetSimpleSpline(drawPoints, 10f);
        return simpledPoints;   
    }
}
