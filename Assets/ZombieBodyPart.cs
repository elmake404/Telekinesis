using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBodyPart : MonoBehaviour, IRopeCollision, IExploded
{
    private float minImpulseToActive = 5f;
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
        currentZombie.isRopeBreak = false;
        currentZombie.isPinned = true;
    }

    public void BreakRope()
    {
        currentZombie.isRopeBreak = true;
        currentZombie.isPinned = false;
        //currentZombie.SetDefaultLayersToAllColliders();
    }

    public void Explode(Vector3 source)
    {
        currentZombie.AddExplosionForceToBody(source);
        currentZombie.EnableRagdoll();
    }

    public void IgnoreRopeColliders(Collider[] colliders)
    {
        currentZombie.IgnoreRopeColliders(colliders);
    }

    private IEnumerator PlaySimpleParticlesOnHit(Vector3 pos, float forceHit)
    {
        GameObject instance = Instantiate(currentZombie.particlesOnHit);
        instance.transform.position = pos;
        instance.transform.localScale = new Vector3(1, 1, 1);
        instance.transform.localScale *= Mathf.Lerp(0.2f, 1f, Mathf.InverseLerp(5, 50, forceHit));
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(particleSystem.main.duration);
        Destroy(instance);
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float force = Mathf.Abs(collision.relativeVelocity.magnitude);
        if (force > minImpulseToActive)
        {
            ContactPoint[] contacts = collision.contacts;
            for (int i = 0; i < contacts.Length; i++)
            {
                StartCoroutine(PlaySimpleParticlesOnHit(contacts[i].point, force));
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
                    currentZombie.connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
                }
            }
        }

        
    }
}
