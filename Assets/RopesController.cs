using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfConnected
{
    zombieBody,
    zombieHead,
    barrelBomb,
    simpleObject,
    staticSimpleObject
}

public class RopesController : MonoBehaviour
{
    public GameObject createRope;
    //[HideInInspector] public List<CreateRope> createdRopes = new List<CreateRope>();

    public void CreateNewRope(List<Vector2> points, ConnectedObject[] connectedObject)
    {
        GameObject newObjCreatedRope = Instantiate(createRope);
        CreateRope createdRope = newObjCreatedRope.GetComponent<CreateRope>();
        createdRope.SetConnectObjectsRigidbodies(connectedObject);  // ok
        createdRope.SetPointsToSpawn(points); // ok
        createdRope.GenerateRopeSections(); // ok
        createdRope.CreateCurrentRope();  // ok
        createdRope.PickObjectsToRope();
        createdRope.ObjectsIgnoreCollisionWithRope();
        createdRope.SetPinToConnected();
        //createdRope.MakeFixRope();
    }


}



public struct ConnectedObject
{
    public Vector3 hitPoint;
    public Rigidbody attacheRigidbody;
    public Collider hitCollider;

    public ConnectedObject(Vector3 hitPoint, Rigidbody rigidbody, Collider hitCollider)
    {
        this.hitPoint = hitPoint;
        this.attacheRigidbody = rigidbody;
        this.hitCollider = hitCollider;
    }
}

public struct ConnectedPin
{
    public CreateRope createRope;
    public int indexConnect;

    public ConnectedPin(CreateRope createRope, int indexConnect)
    {
        this.createRope = createRope;
        this.indexConnect = indexConnect;
    }
}
