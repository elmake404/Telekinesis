using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour, IRopeCollision, IExploded
{
    private ConnectedPin connectedPin;
    public TypeOfConnected selectedType = TypeOfConnected.staticSimpleObject;

    void Start()
    {
        
    }

    public void InitConnect()
    {

    }

    public void BreakRope(Vector3 source)
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
}
