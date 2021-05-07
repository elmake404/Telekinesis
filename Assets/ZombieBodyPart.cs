using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBodyPart : MonoBehaviour, IRopeCollision
{
    private float minImpulseToActive = 20f;
    public CurrentZombieControl currentZombie;
    private ConnectedPin connectedPin;

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    public void InitConnect()
    {
        currentZombie.EnableRagdoll();
    }

    public void BreakRope(Vector3 possourceExplosion)
    {
        
        Debug.Log("EnableRagdoll");

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > minImpulseToActive)
        {
            currentZombie.EnableRagdoll();
        }
    }
}
