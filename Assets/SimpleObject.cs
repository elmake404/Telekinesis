using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleObject : MonoBehaviour, IRopeCollision, IExploded, IInitObject
{
    public Rigidbody objectRigidbody;
    public Collider objectCollider;
    public TypeOfConnected selectedType = TypeOfConnected.simpleObject;
    private ConnectedPin connectedPin;

    private void Start()
    {
        InitEnable();
    }

    public int GetUniqueID()
    {
        return gameObject.GetInstanceID();
    }

    public void InitComponent()
    {
        this.enabled = true;
    }

    private void InitEnable()
    {
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        objectCollider.enabled = true;
    }

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    public TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public void InitConnect()
    {
        return;
    }

    public void Explode(Vector3 source)
    {
        //objectRigidbody.AddExplosionForce(5f, source, 5f, 5f, ForceMode.Impulse);
    }

    public void BreakRope()
    {
        

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {

            if (connectedPin.createRope == null) { return; }
            Debug.Log(collision.collider.gameObject.name);
            ConnectedObject[] objects = connectedPin.createRope.GetConnectedObjects();
            int index = 0;
            if (connectedPin.indexConnect == 0) { index = 1; }
            else { index = 0; }

            if (collision.collider.gameObject.GetComponent<IRopeCollision>().GetUniqueID() == objects[index].uniqueID)
            {
                
                connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
            }
        }


    }

}
