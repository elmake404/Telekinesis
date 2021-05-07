using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRope : MonoBehaviour
{
    private List<Vector2> pointsToSpawn = new List<Vector2>();
    public GameObject ropeSection;
    private List<RopeSection> createdRopeSections = new List<RopeSection>();
    private ConnectedObject[] connectObjects = new ConnectedObject[2];
    private ConfigurableJoint[] jointsConnected = new ConfigurableJoint[2];
    private bool isBreakROpe = false;

    private void Start()
    {
        InitWorkOnConnectedObjects();
    }

    private void FixedUpdate()
    {
        if (isBreakROpe == false)
        {
            if(GetDistanceBeetwenJoints() < 0.5f)
            {
                BreakRope();
                isBreakROpe = true;
            }
        }
        
    }

    public void SetConnectObjectsRigidbodies(ConnectedObject[] connectObject)
    {
        connectObjects[0] = connectObject[0];
        connectObjects[1] = connectObject[1];
    }

    public void SetPointsToSpawn(List<Vector2> points)
    {
        pointsToSpawn = points;
    }

    public void GenerateRopeSections()
    {
        int numOfSections = pointsToSpawn.Count;

        for (int i = 0; i < numOfSections; i++)
        {
            GameObject instanceSection = Instantiate(ropeSection);
            RopeSection instanceRopeSection = instanceSection.GetComponent<RopeSection>();
            createdRopeSections.Add(instanceRopeSection);

            if (i == 0)
            {
                instanceSection.transform.position = connectObjects[0].hitPoint;
            }
            else if (i == numOfSections - 1)
            {
                instanceSection.transform.position = connectObjects[1].hitPoint;
            }
            else
            {
                Ray ray = CameraController.instance.GetRayFromScreen(pointsToSpawn[i]);
                RaycastHit raycastHit;
                bool isHit = Physics.Raycast(ray, out raycastHit);
                float interpolateT = Mathf.InverseLerp(0, pointsToSpawn.Count, i);
                float offsetZ = Mathf.Lerp(Vector3.Distance(ray.origin, connectObjects[0].hitPoint), Vector3.Distance(ray.origin, connectObjects[1].hitPoint), interpolateT);

                if (isHit == true)
                {
                    if (offsetZ > Vector3.Distance(ray.origin, raycastHit.point))
                    {
                        instanceSection.transform.position = raycastHit.point;
                        continue;
                    }
                }
                Vector3 pos = ray.origin + (ray.direction * offsetZ);
                instanceSection.transform.position = pos;
            }
        }
    }

    public void CreateCurrentRope()
    {
        for (int i = 0; i < createdRopeSections.Count; i++)
        {
            SpringJoint springJoint = createdRopeSections[i].springJoint;
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.spring = 300f;

            if (i == createdRopeSections.Count - 1)
            {
                continue;
            }

            //float minDistance = Vector3.Distance(createdRopeSections[i].transform.position, createdRopeSections[i + 1].transform.position);
            //springJoint.minDistance = minDistance;
            springJoint.connectedBody = createdRopeSections[i + 1].sphereRigidbody;
            springJoint.connectedAnchor = Vector3.zero;
        }
    }

    public void PickObjectsToRope()
    {

        ConfigurableJoint joint1 = createdRopeSections[0].transform.gameObject.AddComponent<ConfigurableJoint>();
        jointsConnected[0] = joint1;
        jointsConnected[0].connectedBody = connectObjects[0].attacheRigidbody;
        joint1.xMotion = ConfigurableJointMotion.Locked;
        joint1.yMotion = ConfigurableJointMotion.Locked;
        joint1.zMotion = ConfigurableJointMotion.Locked;
        //joint1.enableCollision = true;

        ConfigurableJoint joint2 = createdRopeSections[createdRopeSections.Count - 1].transform.gameObject.AddComponent<ConfigurableJoint>();
        jointsConnected[1] = joint2;
        jointsConnected[1].connectedBody = connectObjects[1].attacheRigidbody;
        joint2.xMotion = ConfigurableJointMotion.Locked;
        joint2.yMotion = ConfigurableJointMotion.Locked;
        joint2.zMotion = ConfigurableJointMotion.Locked;
        //joint2.enableCollision = true;
    }

    public void SetPinToConnected()
    {
        for (int i = 0; i < connectObjects.Length; i++)
        {
            IRopeCollision ropeCollision = connectObjects[i].attacheRigidbody.gameObject.GetComponent(typeof(IRopeCollision)) as IRopeCollision;
            ropeCollision.SetWithRopeConnected(new ConnectedPin(this, i));
        }

    }

    public void ObjectsIgnoreCollisionWithRope()
    {
        for (int i = 0; i < createdRopeSections.Count; i++)
        {
            Physics.IgnoreCollision(connectObjects[0].hitCollider, createdRopeSections[i].sphereCollider);
            Physics.IgnoreCollision(connectObjects[1].hitCollider, createdRopeSections[i].sphereCollider);
        }
    }

    public void MakeFixRope()
    {
        Rigidbody firstRigidbody = createdRopeSections[0].sphereRigidbody;
        Rigidbody lastRigidbody = createdRopeSections[createdRopeSections.Count - 1].sphereRigidbody;

        firstRigidbody.useGravity = false;
        firstRigidbody.isKinematic = true;

        lastRigidbody.useGravity = false;
        lastRigidbody.isKinematic = true;

    }



    public List<RopeSection> GetCreatedRopeSections()
    {
        return createdRopeSections;
    }

    private void BreakRope()
    {
        jointsConnected[0].connectedBody = null;
        jointsConnected[1].connectedBody = null;
        InitHitConnectedObjects();
        DestroyThisRope();

    }

    private float GetDistanceBeetwenJoints()
    {
        return Vector3.Distance(createdRopeSections[0].transform.position, createdRopeSections[createdRopeSections.Count - 1].transform.position);
    }

    private void InitHitConnectedObjects()
    {
        Vector3 middleRopePos = Vector3.Lerp(createdRopeSections[0].transform.position, createdRopeSections[createdRopeSections.Count - 1].transform.position, 0.5f);
        for (int i = 0; i < connectObjects.Length; i++)
        {
            IRopeCollision ropeCollision = connectObjects[i].attacheRigidbody.gameObject.GetComponent(typeof(IRopeCollision)) as IRopeCollision;
            ropeCollision.BreakRope(middleRopePos);
        }
        
    }

    private void InitWorkOnConnectedObjects()
    {
        for (int i = 0; i < connectObjects.Length; i++)
        {
            IRopeCollision ropeCollision = connectObjects[i].attacheRigidbody.gameObject.GetComponent(typeof(IRopeCollision)) as IRopeCollision;
            ropeCollision.InitConnect();
        }
    }

    private void DestroyThisRope()
    {
        for (int i = 0; i < createdRopeSections.Count; i++)
        {
            Destroy(createdRopeSections[i].gameObject);
        }
        Destroy(gameObject);
    }

    public void ChangeConnectedObjectToPin(Rigidbody bodyToChange, int indexPin)
    {
        jointsConnected[indexPin].connectedBody = bodyToChange;
    }
}

