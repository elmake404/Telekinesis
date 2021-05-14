using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBodyPart : MonoBehaviour, IRopeCollision, IExploded
{
    private float minImpulseToActive = 10f;
    public CurrentZombieControl currentZombie;
    public TypeOfConnected selectedType = TypeOfConnected.zombieBody;
    

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        currentZombie.connectedPin = connectedPin;
        
    }

    public int GetUniqueID()
    {
        return currentZombie.gameObject.GetInstanceID();
    }

    public TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public void InitConnect()
    {
        currentZombie.isPinned = true;
    }

    public void BreakRope()
    {
        currentZombie.isRopeBreak = true;
        currentZombie.isPinned = false;

    }

    public void Explode(Vector3 source)
    {
        currentZombie.AddExplosionForceToBody(source);
        currentZombie.EnableRagdoll();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.impulse.magnitude > minImpulseToActive)
        {
            ContactPoint[] contacts = collision.contacts;
            for (int i = 0; i < contacts.Length; i++)
            {
                currentZombie.PlayParticlesOnHit(contacts[i].point, transform);
            }

            currentZombie.EnableRagdoll();
        }

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
                    Debug.Log(collision.collider.gameObject.GetComponent<IRopeCollision>().GetUniqueID() + "   " + objects[index].uniqueID);

                    currentZombie.connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
                }
            }
        }

        

        
    }
}
