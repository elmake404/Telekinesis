using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBehaviour : MonoBehaviour
{
    private List<Vector2> pointsToSpawn = new List<Vector2>();
    public GameObject ropeSection;
    public GameObject tracer;
    private List<RopeSection> createdRopeSections = new List<RopeSection>();
    private ConnectedObject[] connectObjects = new ConnectedObject[2];
    private ConfigurableJoint[] jointsConnected = new ConfigurableJoint[2];
    private Collider[] ropeColliders;

    private void Start()
    {
        InitWorkOnConnectedObjects();
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
            Physics.IgnoreCollision(connectObjects[0].hitCollider, instanceRopeSection.sphereCollider);
            Physics.IgnoreCollision(connectObjects[1].hitCollider, instanceRopeSection.sphereCollider);
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
        float springForce = 200f;

        for (int i = 0; i < createdRopeSections.Count; i++)
        {
            SpringJoint springJoint = createdRopeSections[i].springJoint;
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.spring = springForce;

            if (i == createdRopeSections.Count - 1)
            {
                Destroy(springJoint);
                continue;
            }

            springJoint.connectedBody = createdRopeSections[i + 1].sphereRigidbody;
            springJoint.connectedAnchor = Vector3.zero;
        }
    }


    public void PickObjectsToRope()
    {
        TypeOfConnected[] typeOfConnects = new TypeOfConnected[2];
        IRopeCollision[] iropeCollisions = new IRopeCollision[2];
        ObjectToPick[] objectToPicks = new ObjectToPick[2];

        

        iropeCollisions[0] = connectObjects[0].attacheRigidbody.gameObject.GetComponent<IRopeCollision>();
        iropeCollisions[1] = connectObjects[1].attacheRigidbody.gameObject.GetComponent<IRopeCollision>();
        objectToPicks[0] = connectObjects[0].hitCollider.transform.GetComponent<ObjectToPick>();
        objectToPicks[1] = connectObjects[1].hitCollider.transform.GetComponent<ObjectToPick>();

        //Debug.Log(connectObjects[0].hitCollider.name);
        //Debug.Log(connectObjects[1].hitCollider.name);

        ConnectedRope[] connectedRopes = new ConnectedRope[2];
        connectedRopes[0] = new ConnectedRope();
        connectedRopes[1] = new ConnectedRope();

        connectedRopes[0].attachedRopeBehaviour = this;      
        connectedRopes[1].attachedRopeBehaviour = this;
        objectToPicks[0].isAttachedToRope = true;
        objectToPicks[1].isAttachedToRope = true;

        objectToPicks[0].AddConnectedRope(connectedRopes[0]);
        objectToPicks[1].AddConnectedRope(connectedRopes[1]);

        typeOfConnects[0] = iropeCollisions[0].GetTypeOfConnected();
        typeOfConnects[1] = iropeCollisions[1].GetTypeOfConnected();


        if (typeOfConnects[0] == TypeOfConnected.staticSimpleObject || typeOfConnects[1] == TypeOfConnected.staticSimpleObject)
        {
            int otherConnectedIndex;
            int indexStaticObject = GetIndexCorrectTypeConnected(typeOfConnects, TypeOfConnected.staticSimpleObject, out otherConnectedIndex);

            connectedRopes[indexStaticObject].orderInRope = indexStaticObject;
            connectedRopes[otherConnectedIndex].orderInRope = otherConnectedIndex;

            switch (typeOfConnects[otherConnectedIndex])
            {
                case TypeOfConnected.none:
                    Debug.LogError("Try connect None object");
                    break;
                case TypeOfConnected.zombieBody:
                case TypeOfConnected.zombieHead:
                case TypeOfConnected.zombieFoot:
                case TypeOfConnected.zombieHand:
                    ZombieBodyControl zombieBodyControl = objectToPicks[otherConnectedIndex] as ZombieBodyControl;
                    zombieBodyControl.zombieControl.InitDeathZombie();
                    UnionObjectWithFixedJoint(otherConnectedIndex);
                    return;
                case TypeOfConnected.barrelBomb:
                case TypeOfConnected.simpleObject:
                    UnionObjectWithFixedJoint(otherConnectedIndex);
                    return;
                case TypeOfConnected.staticSimpleObject:
                    Debug.LogError("Try connect two static objects");
                    return;
            }
        }

        else if (TypeIsPartOfZombieBody(typeOfConnects[0]) || TypeIsPartOfZombieBody(typeOfConnects[1]))
        {
            int otherConnectedIndex;
            int indexBodyZombie = GetIndexZombieBodyTypeOfConnected(typeOfConnects, out otherConnectedIndex);

            connectedRopes[indexBodyZombie].orderInRope = indexBodyZombie;
            connectedRopes[otherConnectedIndex].orderInRope = otherConnectedIndex;

            switch (typeOfConnects[otherConnectedIndex])
            {
                case TypeOfConnected.none:
                    Debug.LogError("Try connect None object");
                    break;
                case TypeOfConnected.zombieBody:
                case TypeOfConnected.zombieHead:
                case TypeOfConnected.zombieFoot:
                case TypeOfConnected.zombieHand:
                    ZombieBodyControl zombieBodyControl_0 = objectToPicks[indexBodyZombie] as ZombieBodyControl;
                    zombieBodyControl_0.zombieControl.InitDeathZombie();
                    ZombieBodyControl zombieBodyControl_1 = objectToPicks[otherConnectedIndex] as ZombieBodyControl;
                    zombieBodyControl_1.zombieControl.InitDeathZombie();
                    UnitonObjectsAtTwoEndsRope();
                    return;
                case TypeOfConnected.barrelBomb:
                case TypeOfConnected.simpleObject:
                    UnionObjectWithFixedJoint(otherConnectedIndex);
                    return;
            }
        }
        else
        {
            connectedRopes[0].orderInRope = 0;
            connectedRopes[1].orderInRope = 1;
            UnitonObjectsAtTwoEndsRope();
        }
    }

    private int GetIndexCorrectTypeConnected(TypeOfConnected[] storeTypes, TypeOfConnected findType, out int otherConnectedIndex)
    {
        if (storeTypes[0] == findType)
        {
            otherConnectedIndex = 1;
            return 0;
        }
        else if (storeTypes[1] == findType)
        {
            otherConnectedIndex = 0;
            return 1;
        }
        else
        {
            otherConnectedIndex = -1;
            return -1;
        }
    }

    private int GetIndexZombieBodyTypeOfConnected(TypeOfConnected[] storeTypes, out int otherConnectedIndex)
    {

        if (TypeIsPartOfZombieBody(storeTypes[0]) == true)
        {
            otherConnectedIndex = 1;
            return 0;
        }
        else if (TypeIsPartOfZombieBody(storeTypes[1]) == true)
        {
            otherConnectedIndex = 0;
            return 1;
        }
        else
        {
            otherConnectedIndex = -1;
            return -1;
        }

    }

    private bool TypeIsPartOfZombieBody(TypeOfConnected type)
    {
        switch (type)
        {
            case TypeOfConnected.zombieBody:
            case TypeOfConnected.zombieHead:
            case TypeOfConnected.zombieFoot:
            case TypeOfConnected.zombieHand:
                return true;
            default:
                return false;
        }
    }

    public void SetRopeColliders()
    {
        Collider[] colliders = new Collider[createdRopeSections.Count];
        
        for(int i = 0; i< colliders.Length; i++)
        {
            colliders[i] = createdRopeSections[i].sphereCollider;
        }
        ropeColliders = colliders;
    }

    private void UnitonObjectsAtTwoEndsRope()
    {
        ConfigurableJoint joint1 = createdRopeSections[0].transform.gameObject.AddComponent<ConfigurableJoint>();
        jointsConnected[0] = joint1;
        jointsConnected[0].connectedBody = connectObjects[0].attacheRigidbody;
        joint1.xMotion = ConfigurableJointMotion.Locked;
        joint1.yMotion = ConfigurableJointMotion.Locked;
        joint1.zMotion = ConfigurableJointMotion.Locked;

        ConfigurableJoint joint2 = createdRopeSections[createdRopeSections.Count - 1].transform.gameObject.AddComponent<ConfigurableJoint>();
        jointsConnected[1] = joint2;
        jointsConnected[1].connectedBody = connectObjects[1].attacheRigidbody;
        joint2.xMotion = ConfigurableJointMotion.Locked;
        joint2.yMotion = ConfigurableJointMotion.Locked;
        joint2.zMotion = ConfigurableJointMotion.Locked;
    }

    private void UnionObjectWithFixedJoint(int indexNonFixed)
    {
        int indexRopeSection = 0;
        if (indexNonFixed == 0) { indexRopeSection = 0; }
        else { indexRopeSection = createdRopeSections.Count - 1; }

        ConfigurableJoint joint1 = createdRopeSections[indexRopeSection].gameObject.AddComponent<ConfigurableJoint>();
        jointsConnected[0] = joint1;
        jointsConnected[0].connectedBody = connectObjects[indexNonFixed].attacheRigidbody;
        joint1.xMotion = ConfigurableJointMotion.Locked;
        joint1.yMotion = ConfigurableJointMotion.Locked;
        joint1.zMotion = ConfigurableJointMotion.Locked;

        int indexFixed = 0;
        if (indexNonFixed == 0) { indexFixed = 1; }
        else { indexFixed = 0; }

        int indexOtherEndSectionRope = 0;
        if (indexRopeSection == 0)
        {
            indexOtherEndSectionRope = createdRopeSections.Count - 1;
        }
        else { indexOtherEndSectionRope = 0; }

        ConfigurableJoint joint2 = connectObjects[indexFixed].attacheRigidbody.gameObject.AddComponent<ConfigurableJoint>();
        jointsConnected[1] = joint2;
        jointsConnected[1].connectedBody = createdRopeSections[indexOtherEndSectionRope].sphereRigidbody;
        joint2.xMotion = ConfigurableJointMotion.Locked;
        joint2.yMotion = ConfigurableJointMotion.Locked;
        joint2.zMotion = ConfigurableJointMotion.Locked;

        
    }

    public void SetNewRigidbodyToSelectedJoint(int order, Rigidbody newRigidbody)
    {
        int indexRopeSection;
        if (order == 0) { indexRopeSection = 0; } else { indexRopeSection = createdRopeSections.Count - 1; }
        ConfigurableJoint configurableJoint;
        bool isAttachedConfigurableJoint = createdRopeSections[indexRopeSection].TryGetComponent<ConfigurableJoint>(out configurableJoint);
        if (isAttachedConfigurableJoint == false)
        {
            ConfigurableJoint createdConfigurableJoint = createdRopeSections[indexRopeSection].gameObject.AddComponent<ConfigurableJoint>();
            createdConfigurableJoint.connectedBody = newRigidbody;
            createdConfigurableJoint.xMotion = ConfigurableJointMotion.Locked;
            createdConfigurableJoint.yMotion = ConfigurableJointMotion.Locked;
            createdConfigurableJoint.zMotion = ConfigurableJointMotion.Locked;
            jointsConnected[order] = createdConfigurableJoint;
        }
        else
        {
            configurableJoint.connectedBody = newRigidbody;
        }
    }

    public void SetPinToConnected()
    {
        for (int i = 0; i < connectObjects.Length; i++)
        {
            IRopeCollision ropeCollision = connectObjects[i].attacheRigidbody.gameObject.GetComponent(typeof(IRopeCollision)) as IRopeCollision;
            ropeCollision.SetWithRopeConnected(new ConnectedPin(this, i));
            connectObjects[i].uniqueID = ropeCollision.GetUniqueID();
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

    public void BreakRope()
    {
        jointsConnected[0].connectedBody = null;
        jointsConnected[0].xMotion = ConfigurableJointMotion.Free;
        jointsConnected[0].yMotion = ConfigurableJointMotion.Free;
        jointsConnected[0].zMotion = ConfigurableJointMotion.Free;

        jointsConnected[1].connectedBody = null;
        jointsConnected[1].xMotion = ConfigurableJointMotion.Free;
        jointsConnected[1].yMotion = ConfigurableJointMotion.Free;
        jointsConnected[1].zMotion = ConfigurableJointMotion.Free;
        InitHitConnectedObjects();
        DestroyThisRope();

    }


    private void InitHitConnectedObjects()
    {//Vector3 middleRopePos = Vector3.Lerp(createdRopeSections[0].transform.position, createdRopeSections[createdRopeSections.Count - 1].transform.position, 0.5f);
        for (int i = 0; i < connectObjects.Length; i++)
        {
            IRopeCollision ropeCollision = connectObjects[i].attacheRigidbody.gameObject.GetComponent(typeof(IRopeCollision)) as IRopeCollision;
            ropeCollision.BreakRope();
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

    public ConnectedObject[] GetConnectedObjects()
    {
        return connectObjects;
    }
}

