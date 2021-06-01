using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieFootPart : ZombieBodyControl
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


    new private void OnJointBreak(float breakForce)
    {
        //base.OnJointBreak(breakForce);
        zombieControl.InitBreakJoint(zombieBodyPartID, transform.GetComponent<ZombieBodyControl>());
    }

    new private void OnCollisionEnter(Collision collision)
    {
        if (IsIgnoreCollision(collision)) { return; }
        Debug.Log(collision.collider.name);

        float force = GetRelativeVelocityMagnitude(collision);

        if (force > minForce)
        {
            zombieControl.InitDeathZombie();
        }
    }
}
