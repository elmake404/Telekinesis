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
        
        /*
        if (currentZombie.isRopeBreak == true) { return; }
        if (currentZombie.isPinned == true)
        {
            if (collision.gameObject.layer == 8)
            {
                //if (currentZombie.connectedPin.createRope == null) { return; }
                ConnectedObject[] objects = currentZombie.connectedPin.createRope.GetConnectedObjects();
                int index = 0;
                if (currentZombie.connectedPin.indexConnect == 0) { index = 1; }
                else { index = 0; }
                

                if (collision.collider.gameObject.GetComponent<IRopeCollision>().GetUniqueID() == objects[index].uniqueID)
                {
                    currentZombie.connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
                }
            }
        }
        */
        
    }
}
