using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHeadPart : ZombieBodyControl
{
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Start()
    {
        base.Start();
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
    

    new private void OnJointBreak(float breakForce)
    {
        //base.OnJointBreak(breakForce);
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
