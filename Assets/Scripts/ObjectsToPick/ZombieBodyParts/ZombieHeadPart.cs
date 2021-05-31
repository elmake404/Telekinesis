using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHeadPart : ZombieBodyControl
{
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override int GetUniqueID()
    {
        return zombieControl.gameObject.GetInstanceID();
        
    }

    public override TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public void Explode()
    {
        
    }

    public override void InitConnect()
    {
        
    }

    public override void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        zombieControl.connectedPin = connectedPin;
    }

    public override void BreakRope()
    {   

    }

    public override void Explode(Vector3 source)
    {
        
    }

    public void IgnoreRopeColliders(Collider[] colliders)
    {
        zombieControl.IgnoreRopeColliders(colliders);
    }
    

    protected override void OnJointBreak(float breakForce)
    {
        base.OnJointBreak(breakForce);
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
