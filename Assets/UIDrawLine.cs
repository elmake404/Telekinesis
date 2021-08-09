﻿using System.Collections;
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
    private TypeOfConnected[] typeOfConnecteds = new TypeOfConnected[2];
    private GameObject[] raycastedObjects = new GameObject[2];

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

        if (isHit == true)
        {
            IRopeCollision ropeCollision = raycastHit.collider.gameObject.GetComponent<IRopeCollision>();
            TypeOfConnected typeOfConnected = ropeCollision.GetTypeOfConnected();
            raycastedObjects[0] = raycastHit.collider.gameObject;
            typeOfConnecteds[0] = typeOfConnected;

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

        if (isHit)
        {
            IRopeCollision ropeCollision = raycastHit.collider.gameObject.GetComponent<IRopeCollision>();
            TypeOfConnected typeOfConnected = ropeCollision.GetTypeOfConnected();
            raycastedObjects[1] = raycastHit.collider.gameObject;
            typeOfConnecteds[1] = typeOfConnected;

            bool isSimpleObjectAttached = typeOfConnecteds[0] == TypeOfConnected.simpleObject | typeOfConnecteds[1] == TypeOfConnected.simpleObject? (typeOfConnecteds[0] == typeOfConnecteds[1]? false : true) : false;
            if (isSimpleObjectAttached)
            {
                System.Func<TypeOfConnected, bool> isZombieBodyPart = delegate (TypeOfConnected type)
                {
                    bool result = false;
                    switch (type)
                    {
                        case TypeOfConnected.none:
                            break;
                        case TypeOfConnected.zombieBody:
                        case TypeOfConnected.zombieHead:
                        case TypeOfConnected.zombieFoot:
                        case TypeOfConnected.zombieHand:
                            result = true;
                            break;
                        case TypeOfConnected.barrelBomb:
                            break;
                        case TypeOfConnected.simpleObject:
                            break;
                        case TypeOfConnected.staticSimpleObject:
                            break;
                    }
                    return result;
                };
                
                bool isFirstConnect = isZombieBodyPart.Invoke(typeOfConnecteds[0]);
                bool isSecondConnect = isZombieBodyPart.Invoke(typeOfConnecteds[1]);

                System.Action<int> actionIfZombieBody = delegate (int index)
                {
                    ZombieBodyControl zombieBodyControl = raycastedObjects[index == 0 ? 1 : 0].GetComponent<ZombieBodyControl>();
                    zombieBodyControl.zombieControl.AddExpectedObjectToCollision(uniqueIDStorage[index]);
                };

                if (isFirstConnect) { actionIfZombieBody.Invoke(1); }
                else if (isSecondConnect) { actionIfZombieBody.Invoke(0); }
            }
            

            uniqueIDStorage[1] = ropeCollision.GetUniqueID();
            //Debug.Log(uniqueIDStorage[0] + "   " + uniqueIDStorage[1]);

            if (uniqueIDStorage[0] != uniqueIDStorage[1])
            {
                connectedObjects[1] = new ConnectedObject(raycastHit.point, raycastHit.rigidbody, raycastHit.collider);
                _ropesController.CreateNewRope(GetSimpleSpline(), connectedObjects);
                
            }
        }
        else
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
