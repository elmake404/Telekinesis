using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRope : MonoBehaviour
{
    private List<Vector2> pointsToSpawn = new List<Vector2>();
    public GameObject ropeSection;
    public GameObject tracer;
    private List<RopeSection> createdRopeSections = new List<RopeSection>();
    private ConnectedObject[] connectObjects = new ConnectedObject[2];
    private ConfigurableJoint[] jointsConnected = new ConfigurableJoint[2];
    private bool isBreakROpe = false;
    private int numOfDetectingCollision = 0;
    private Coroutine coroutineTimerToBreakRope;
    private Collider[] ropeColliders;

    private void Start()
    {
        InitWorkOnConnectedObjects();
        
        //TimerToDestreoyRope();
        //coroutineTimerToBreakRope = StartCoroutine(TimerToDestreoyRope());

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

            if (i == createdRopeSections.Count - 1)
            {
                Destroy(springJoint);
                continue;
            }

            //float minDistance = Vector3.Distance(createdRopeSections[i].transform.position, createdRopeSections[i + 1].transform.position);
            //springJoint.minDistance = minDistance;
            springJoint.connectedBody = createdRopeSections[i + 1].sphereRigidbody;
            springJoint.connectedAnchor = Vector3.zero;
        }
    }

    private void SetTracerToRopePart(Transform transform)
    {
        GameObject instance = Instantiate(tracer, transform);
    }

    public void PickObjectsToRope()
    {
        TypeOfConnected[] typeOfConnects = new TypeOfConnected[2];
        IRopeCollision[] iropeCollisions = new IRopeCollision[2];
        iropeCollisions[0] = connectObjects[0].attacheRigidbody.gameObject.GetComponent<IRopeCollision>();
        iropeCollisions[1] = connectObjects[1].attacheRigidbody.gameObject.GetComponent<IRopeCollision>();

        typeOfConnects[0] = iropeCollisions[0].GetTypeOfConnected();
        typeOfConnects[1] = iropeCollisions[1].GetTypeOfConnected();


        if (typeOfConnects[0] == TypeOfConnected.zombieBody || typeOfConnects[1] == TypeOfConnected.zombieBody)
        {
            
            if (typeOfConnects[0] == TypeOfConnected.zombieBody && typeOfConnects[1] == TypeOfConnected.zombieBody)
            {
                UnitonObjectsAtTwoEndsRope();
                SetTracerToRopePart(createdRopeSections[0].transform);
                SetTracerToRopePart(createdRopeSections[createdRopeSections.Count - 1].transform);

                ZombieBodyPart zombieBodyPart_0 = connectObjects[0].attacheRigidbody.transform.GetComponent<ZombieBodyPart>();
                zombieBodyPart_0.currentZombie.EnableRagdoll();
                zombieBodyPart_0.currentZombie.IgnoreRopeColliders(ropeColliders);

                ZombieBodyPart zombieBodyPart_1 = connectObjects[1].attacheRigidbody.transform.GetComponent<ZombieBodyPart>();
                zombieBodyPart_1.currentZombie.EnableRagdoll();
                zombieBodyPart_1.currentZombie.IgnoreRopeColliders(ropeColliders);
                return;
            }

            else if (typeOfConnects[0] == TypeOfConnected.zombieHead || typeOfConnects[1] == TypeOfConnected.zombieHead)
            {
                int index = 0;
                int indexRopeSection = 0;
                if (typeOfConnects[0] == TypeOfConnected.zombieHead)
                {
                    index = 0;
                    indexRopeSection = 0;
                }
                else
                {
                    index = 1;
                    indexRopeSection = createdRopeSections.Count - 1;
                }

                ZombieHeadPart zombieHeadPart = connectObjects[index].attacheRigidbody.transform.GetComponent<ZombieHeadPart>();
                zombieHeadPart.zombieControl.EnableRagdoll();
                zombieHeadPart.IgnoreRopeColliders(ropeColliders);

                SetTracerToRopePart(createdRopeSections[indexRopeSection].transform);

                int index_2 = 0;
                if (index == 0) { index_2 = 1; } else { index_2 = 0; }

                ZombieBodyPart zombieBodyPart = connectObjects[index_2].attacheRigidbody.transform.GetComponent<ZombieBodyPart>();
                zombieBodyPart.currentZombie.EnableRagdoll();
                zombieBodyPart.IgnoreRopeColliders(ropeColliders);
                UnitonObjectsAtTwoEndsRope();
            }

            else if (typeOfConnects[0] == TypeOfConnected.staticSimpleObject || typeOfConnects[1] == TypeOfConnected.staticSimpleObject)
            {
                int index = 0;
                int indexRopeSection = 0;
                if (typeOfConnects[0] == TypeOfConnected.zombieBody)
                {
                    index = 0;
                    indexRopeSection = 0;
                }
                else
                {
                    index = 1;
                    indexRopeSection = createdRopeSections.Count - 1;
                }

                UnionObjectWithFixedJoint(index);
                SetTracerToRopePart(createdRopeSections[indexRopeSection].transform);
                ZombieBodyPart zombieBodyPart = connectObjects[index].attacheRigidbody.transform.GetComponent<ZombieBodyPart>();
                zombieBodyPart.currentZombie.EnableRagdoll();
                zombieBodyPart.IgnoreRopeColliders(ropeColliders);
                return;
            }

            else
            {
                int index = 0;
                int indexZombieBody = 0;
                int indexRopeSection = 0;
                if (typeOfConnects[0] == TypeOfConnected.zombieBody) 
                { 
                    index = 1;
                    indexZombieBody = 0;
                    indexRopeSection = createdRopeSections.Count - 1;
                } 
                else 
                { 
                    index = 0;
                    indexZombieBody = 1;
                    indexRopeSection = 0;
                }

                ZombieBodyPart zombieBodyPart = connectObjects[indexZombieBody].attacheRigidbody.transform.GetComponent<ZombieBodyPart>();
                zombieBodyPart.IgnoreRopeColliders(ropeColliders);

                UnionObjectWithFixedJoint(index);
                SetTracerToRopePart(createdRopeSections[indexRopeSection].transform);
                return;
            }
        }

        if (typeOfConnects[0] == TypeOfConnected.zombieHead || typeOfConnects[1] == TypeOfConnected.zombieHead)
        {
            
            if (typeOfConnects[0] == TypeOfConnected.zombieHead && typeOfConnects[1] == TypeOfConnected.zombieHead)
            {
                UnitonObjectsAtTwoEndsRope();
                SetTracerToRopePart(createdRopeSections[0].transform);
                SetTracerToRopePart(createdRopeSections[createdRopeSections.Count - 1].transform);

                ZombieHeadPart zombieHeadPart_0 = connectObjects[0].attacheRigidbody.transform.GetComponent<ZombieHeadPart>();
                ZombieHeadPart zombieHeadPart_1 = connectObjects[1].attacheRigidbody.transform.GetComponent<ZombieHeadPart>();

                zombieHeadPart_0.zombieControl.EnableRagdoll();
                zombieHeadPart_0.IgnoreRopeColliders(ropeColliders);
                zombieHeadPart_1.zombieControl.EnableRagdoll();
                zombieHeadPart_1.IgnoreRopeColliders(ropeColliders);
                return;
            }


            else if (typeOfConnects[0] == TypeOfConnected.staticSimpleObject || typeOfConnects[1] == TypeOfConnected.staticSimpleObject)
            {
                int index = 0;
                int indexRopeSection = 0;

                if (typeOfConnects[0] == TypeOfConnected.zombieHead)
                {
                    index = 0;
                    indexRopeSection = 0;
                }
                else
                {
                    index = 1;
                    indexRopeSection = createdRopeSections.Count - 1;
                }

                UnionObjectWithFixedJoint(index);
                SetTracerToRopePart(createdRopeSections[indexRopeSection].transform);

                ZombieHeadPart zombieHeadPart = connectObjects[index].attacheRigidbody.transform.GetComponent<ZombieHeadPart>();
                zombieHeadPart.zombieControl.EnableRagdoll();
                zombieHeadPart.IgnoreRopeColliders(ropeColliders);
                return;
            }

            else
            {
                int index = 0;
                int indexZombieHead = 0;
                int indexRopeSection = 0;

                if (typeOfConnects[0] == TypeOfConnected.zombieHead) 
                { 
                    index = 1;
                    indexZombieHead = 0;
                    indexRopeSection = createdRopeSections.Count - 1;
                } 
                else 
                { 
                    index = 0;
                    indexZombieHead = 1;
                    indexRopeSection = 0;
                }

                ZombieHeadPart zombieHeadPart = connectObjects[indexZombieHead].attacheRigidbody.transform.GetComponent<ZombieHeadPart>();
                zombieHeadPart.IgnoreRopeColliders(ropeColliders);

                SetTracerToRopePart(createdRopeSections[indexRopeSection].transform);
                UnionObjectWithFixedJoint(index);
                return;
            }

        }
        else
        {
            SetTracerToRopePart(createdRopeSections[0].transform);
            SetTracerToRopePart(createdRopeSections[createdRopeSections.Count - 1].transform);
            UnitonObjectsAtTwoEndsRope();
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

        ConfigurableJoint joint1 = createdRopeSections[indexRopeSection].transform.gameObject.AddComponent<ConfigurableJoint>();
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

    public void SetPinToConnected()
    {
        for (int i = 0; i < connectObjects.Length; i++)
        {
            IRopeCollision ropeCollision = connectObjects[i].attacheRigidbody.gameObject.GetComponent(typeof(IRopeCollision)) as IRopeCollision;
            ropeCollision.SetWithRopeConnected(new ConnectedPin(this, i));
            connectObjects[i].uniqueID = ropeCollision.GetUniqueID();
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

    private IEnumerator TimerToDestreoyRope()
    {
        yield return new WaitForSeconds(5f);
        BreakRope();
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
    {
        Vector3 middleRopePos = Vector3.Lerp(createdRopeSections[0].transform.position, createdRopeSections[createdRopeSections.Count - 1].transform.position, 0.5f);
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

    public void ManualBreakRopeIfConnectedObjCollided()
    {
        numOfDetectingCollision += 1;
        if(numOfDetectingCollision >= 2)
        {
            //StopCoroutine(coroutineTimerToBreakRope);
            BreakRope();
        }
    }

    public ConnectedObject[] GetConnectedObjects()
    {
        return connectObjects;
    }
}

