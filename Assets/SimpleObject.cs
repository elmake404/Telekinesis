using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleObject : MonoBehaviour, IRopeCollision
{
    public Rigidbody objectRigidbody;
    public Collider objectCollider;
    private ConnectedPin connectedPin;

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    public void InitConnect()
    {
        return;
    }

    public void BreakRope(Vector3 sourceExplosion)
    {
        Vector3 clotsestPos = objectCollider.ClosestPoint(sourceExplosion);
        Vector3 forceDir = (clotsestPos - sourceExplosion).normalized;
        objectRigidbody.AddForce(clotsestPos + 5f * forceDir, ForceMode.Impulse);

    }


}
