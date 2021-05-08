using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleObject : MonoBehaviour, IRopeCollision, IExploded
{
    public Rigidbody objectRigidbody;
    public Collider objectCollider;
    public TypeOfConnected selectedType = TypeOfConnected.simpleObject;
    private ConnectedPin connectedPin;

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
        objectRigidbody.AddExplosionForce(5f, source, 5f, 5f, ForceMode.Impulse);
    }

    public void BreakRope(Vector3 sourceExplosion)
    {
        Vector3 clotsestPos = objectCollider.ClosestPoint(sourceExplosion);
        Vector3 forceDir = (clotsestPos - sourceExplosion).normalized;
        objectRigidbody.AddForce(clotsestPos + 5f * forceDir, ForceMode.Impulse);

    }


}
