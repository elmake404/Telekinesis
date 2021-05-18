using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrawLine : MonoBehaviour
{
    public UIMeshRenderer uIMeshRenderer;
    public float widthLine;
    public float minStepLine;
    public float maxRopeLength;
    private List<Vector2> drawPoints = new List<Vector2>();
    private ConnectedObject[] connectedObjects = new ConnectedObject[2];
    public LayerMask hitToOnject;
    public RectTransform canvasRectTransform;
    public Canvas canvas;
    private SlowMotionControl slowMotionControl;
    private float drawLineLength = 0f;

    private void Start()
    {
        slowMotionControl = GeneralManager.instance.slowMotionControl;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Input.mousePosition);
            if (slowMotionControl.isSlowMotion == true) { return; }
            CheckEntryRaycast();
        }

        if (Input.GetMouseButton(0))
        {
            if (drawLineLength >= maxRopeLength) { return; }
            AddDrawPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            CheckOutroRaycast();
            drawLineLength = 0f;
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
            Vector2 adjustPoint = point;
            
            adjustPoint = point - new Vector2(canvasRectTransform.position.x, canvasRectTransform.position.y);
            adjustPoint /= canvas.scaleFactor;
            //adjustPoint = point - Vector2.Lerp(Vector2.zero, new Vector2(540, 960), canvasRectTransform.localScale.magnitude);
            drawLineLength += minStepLine;
            
            drawPoints.Add(point);
            uIMeshRenderer.meshedPoints.AddPoint(adjustPoint);
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
