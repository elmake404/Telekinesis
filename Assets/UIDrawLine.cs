using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrawLine : MonoBehaviour
{
    public UIMeshRenderer uIMeshRenderer;
    public RopeLengthController ropeLengthController;
    public float widthLine;
    public float minStepLine;
    public float maxRopeLength;
    private List<Vector2> drawPoints = new List<Vector2>();
    private ConnectedObject[] connectedObjects = new ConnectedObject[2];
    public LayerMask hitToOnject;
    public RectTransform canvasRectTransform;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private SlowMotionBehaviour _slowMotionControl;
    [SerializeField] private Camera _camera;
    [SerializeField] private RopesController _ropesController;
    private float drawLineLength = 0f;
    private int[] uniqueIDStorage = new int[2];
    


    private void Start()
    {
        
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (_slowMotionControl.isSlowMotion == true) { return; }
            CheckEntryRaycast();
        }

        if (Input.GetMouseButton(0))
        {
            if (drawLineLength >= maxRopeLength) 
            {
                CheckOutroRaycast();
                drawLineLength = 0f;
                return;
            }
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
            drawLineLength += distance;
            int substractRope = Mathf.RoundToInt(Mathf.Lerp(0, 100, Mathf.InverseLerp(0, maxRopeLength, drawLineLength)));
            ropeLengthController.SetFillSpiral(100 - substractRope);

            drawPoints.Add(point);
            uIMeshRenderer.meshedPoints.AddPoint(GetUIScaledPoint(point));
            uIMeshRenderer.UpdateMesh();
            return;
        }
        
    }

    private Vector2 GetUIScaledPoint(Vector2 screenPoint)
    {
        screenPoint = screenPoint - new Vector2(canvasRectTransform.position.x, canvasRectTransform.position.y);
        return screenPoint /= _canvas.scaleFactor;
    }

    private void CheckEntryRaycast()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        //bool isHit = Physics.Raycast(ray, out raycastHit, 100f, hitToOnject);
        bool isHit = Physics.SphereCast(ray.origin, 0.5f, ray.direction, out raycastHit, 100f, hitToOnject);
        //Debug.Log(raycastHit.collider.gameObject.name);
        //Debug.DrawRay(ray.origin + 3f * ray.direction, raycastHit.point, Color.red, Mathf.Infinity);

        if (isHit == true)
        {
            IRopeCollision ropeCollision = raycastHit.collider.gameObject.GetComponent<IRopeCollision>();
            uniqueIDStorage[0] = ropeCollision.GetUniqueID();
            _slowMotionControl.StopTime();
            //postprocessing.EnableEffect();
            connectedObjects[0] = new ConnectedObject(raycastHit.point, raycastHit.rigidbody, raycastHit.collider);
            drawPoints.Add(Input.mousePosition);
            uIMeshRenderer.meshedPoints.AddPoint(GetUIScaledPoint(Input.mousePosition));
            uIMeshRenderer.UpdateMesh();
            ropeLengthController.EnableSensor();
        }
        
        return;
    }

    private void CheckOutroRaycast()
    {
        ropeLengthController.DisableSensor();

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        //bool isHit = Physics.Raycast(ray, out raycastHit, 100f, hitToOnject);
        bool isHit = Physics.SphereCast(ray.origin, 0.5f, ray.direction, out raycastHit, 100f, hitToOnject);
        //Debug.DrawLine(ray.origin + 3f * ray.direction, raycastHit.point, Color.red, Mathf.Infinity);
        //Debug.Log(raycastHit.collider.gameObject.name);

        if (isHit == true)
        {
            IRopeCollision ropeCollision = raycastHit.collider.gameObject.GetComponent<IRopeCollision>();
            uniqueIDStorage[1] = ropeCollision.GetUniqueID();
            Debug.Log(uniqueIDStorage[0] + "   " + uniqueIDStorage[1]);

            if (uniqueIDStorage[0] != uniqueIDStorage[1])
            {
                
                connectedObjects[1] = new ConnectedObject(raycastHit.point, raycastHit.rigidbody, raycastHit.collider);
                _ropesController.CreateNewRope(GetSimpleSpline(), connectedObjects);
                
            }
        }
        else if (isHit == false)
        {
            uIMeshRenderer.meshedPoints.Clear();
            uIMeshRenderer.UpdateMesh();
        }

        uIMeshRenderer.meshedPoints.Clear();
        uIMeshRenderer.UpdateMesh();
        drawPoints.Clear();
        _slowMotionControl.ContinueTime();
        //postprocessing.DisableEffect();
    }

    private List <Vector2> GetSimpleSpline()
    {
        List<Vector2> simpledPoints = DuglasKeper.GetSimpleSpline(drawPoints, 10f);
        return simpledPoints;   
    }
}
