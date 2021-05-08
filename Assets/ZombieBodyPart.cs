using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBodyPart : MonoBehaviour, IRopeCollision, IExploded
{
    private float minImpulseToActive = 3f;
    public CurrentZombieControl currentZombie;
    private ConnectedPin connectedPin;
    public TypeOfConnected selectedType = TypeOfConnected.zombieBody;

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

    }

    public void BreakRope(Vector3 possourceExplosion)
    {
        

    }

    public void Explode(Vector3 source)
    {
        currentZombie.AddExplosionForceToBody(source);
        currentZombie.EnableRagdoll();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > minImpulseToActive)
        {
            currentZombie.EnableRagdoll();
        }

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
