using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBodyControl : ObjectToPick
{
    public ZombieBehaviour zombieControl;
    public bool isBreakingJoint;
    public ZombieBodyPartID zombieBodyPartID;
    public Collider attachedCollider;
    public Rigidbody attachedRigidbody;
    [HideInInspector] public bool isAttachCharacterJoint;
    protected float minForce = 5.0f;
    //[HideInInspector] public ZombieBodyControl thisZombieBodyControl;
    protected CharacterJoint attachedCharacterJoint;

    protected override void OnEnable()
    {
        isAttachCharacterJoint = TryGetComponent<CharacterJoint>(out attachedCharacterJoint);
        connectedRopes = new List<ConnectedRope>();
    }

    protected override void Start()
    {
        
    }

    protected override void OnJointBreak(float breakForce)
    {
        zombieControl.InitBreakJoint(zombieBodyPartID, this);
    }


    public void DestroyAttachedCharacterJoint()
    {
        if (isAttachCharacterJoint == true)
        {
            Destroy(attachedCharacterJoint);
        }
        
    }

    public void DiactivateAttachedCollider()
    {
        attachedCollider.enabled = false;
    }

    public void DiactivateAttachedRigidbody()
    {
        attachedRigidbody.useGravity = false;
        attachedRigidbody.isKinematic = true;
    }

    public void DestroyAttachedCollider()
    {
        Destroy(attachedCollider);
    }

    public void DestroyAttachedRigidbody()
    {
        Destroy(attachedRigidbody);
    }


}
