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

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == 8)
        {
            if (connectedPin.createRope == null) { return; }
            ConnectedObject[] objects = connectedPin.createRope.GetConnectedObjects();
            int index = 0;
            if (connectedPin.indexConnect == 0) { index = 1; }
            else { index = 0; }

            if (collision.collider.gameObject.GetHashCode() == objects[index].attacheRigidbody.gameObject.GetHashCode())
            {
                connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
            }
        }


    }

}
