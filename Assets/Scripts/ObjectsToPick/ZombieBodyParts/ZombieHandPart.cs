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
        //Debug.Log(collision.collider.name);
        
        if (IsIntersectOtherObjectToPick(collision) == true) { return; }
        float force = GetRelativeVelocityMagnitude(collision);

        /*if (force > minForce)
        {
            zombieControl.InitDeathZombie();
        }*/

        ContactPoint[] contactPoints = new ContactPoint[10];
        int numOfContacts = collision.GetContacts(contactPoints);
        for (int i = 0; i < numOfContacts; i++)
        {
            if (contactPoints[i].otherCollider.gameObject.layer == 8)
            {
                if (contactPoints[i].otherCollider.attachedRigidbody.velocity.magnitude > 5f)
                {
                    zombieControl.InitDeathZombie();
                    break;
                }
                else if (expectedCollisionObjectID == contactPoints[i].otherCollider.gameObject.GetHashCode())
                {
                    zombieControl.InitDeathZombie();
                    break;
                }
            }

        }
    }
}
