using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour, IRopeCollision, IExploded, IInitObject
{
    public Collider thisCollider;
    private ConnectedPin connectedPin;
    public TypeOfConnected selectedType = TypeOfConnected.staticSimpleObject;

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
        thisCollider.enabled = true;
    }

    public void InitConnect()
    {

    }

    public void BreakRope()
    {
        
    }
    public void Explode(Vector3 source)
    {

    }

    public TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if (connectedPin.createRope == null) { return; }
            ConnectedObject[] objects = connectedPin.createRope.GetConnectedObjects();
            int index = 0;
            if (connectedPin.indexConnect == 0) { index = 1; }
            else { index = 0; }

            
            if (collision.collider.gameObject.GetComponent<IRopeCollision>().GetUniqueID() == objects[index].uniqueID)
            {
                Debug.Log(collision.collider.gameObject.GetComponent<IRopeCollision>().GetUniqueID() + "   " + objects[index].uniqueID);
                connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
            }
        }
    }
}
