using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBodyPart : ZombieBodyControl
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        zombieControl.connectedPin = connectedPin;
    }

    public override int GetUniqueID()
    {
        return zombieControl.gameObject.GetInstanceID();
    }

    public override TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public override void InitConnect()
    {
        zombieControl.isRopeBreak = false;
        zombieControl.isPinned = true;
    }

    public override void BreakRope()
    {
        zombieControl.isRopeBreak = true;
        zombieControl.isPinned = false;
    }

    public override void Explode(Vector3 source)
    {
        
    }

    public void IgnoreRopeColliders(Collider[] colliders)
    {
        
    }
    
    protected override void OnCollisionEnter(Collision collision)
    {
        float force = GetRelativeVelocityMagnitude(collision);

        if (force > minForce)
        {
            zombieControl.InitDeathZombie();
        }
        
    }
}
