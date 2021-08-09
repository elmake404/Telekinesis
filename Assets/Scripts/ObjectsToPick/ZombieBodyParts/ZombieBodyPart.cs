using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBodyPart : ZombieBodyControl
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void Start()
    {
        base.Start();
        
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

    public void AddExpectedObjectToTheCollision(int id)
    {
        zombieControl.AddExpectedObjectToCollision(id);
    }

    public void IgnoreRopeColliders(Collider[] colliders)
    {
        
    }
    
    new private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.collider.name);
        if (IsIntersectOtherObjectToPick(collision) == true) { return; }
        float force = GetRelativeVelocityMagnitude(collision);

        /*if (force > minForce)
        {
            if (collision.collider.gameObject.layer == 8)
            {
                return;
            }
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
