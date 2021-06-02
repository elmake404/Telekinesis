using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHandPart : ZombieBodyControl
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Start()
    {
        base.Start();
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


    new private void OnJointBreak(float breakForce)
    {
        zombieControl.InitBreakJoint(zombieBodyPartID, transform.GetComponent<ZombieBodyControl>());
    }

    new private void OnCollisionEnter(Collision collision)
    {
        float force = GetRelativeVelocityMagnitude(collision);


        if (force > minForce)
        {
            zombieControl.InitDeathZombie();
        }
    }
}
