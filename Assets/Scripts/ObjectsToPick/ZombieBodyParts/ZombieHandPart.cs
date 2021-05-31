using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHandPart : ZombieBodyControl
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public override int GetUniqueID()
    {
        return zombieControl.GetHashCode();
    }
    public override void InitConnect()
    {
        
    }
    public override void BreakRope()
    {
        
    }
    public override void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        
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
