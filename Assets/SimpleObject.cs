using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleObject : MonoBehaviour, IRopeCollision, IExploded, IInitObject
{
    public Rigidbody objectRigidbody;
    public Collider objectCollider;
    public GameObject interactParticles;
    public TypeOfConnected selectedType = TypeOfConnected.simpleObject;
    private ConnectedPin connectedPin;
    

    private void Start()
    {
        InitEnable();
    }

    public int GetUniqueID()
    {
        return gameObject.GetInstanceID();
    }

    public void InitComponent()
    {
        this.enabled = true;
    }

    private void InitEnable()
    {
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        objectCollider.enabled = true;
    }

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    public TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public void InitConnect()
    {
        return;
    }

    public void Explode(Vector3 source)
    {
        //objectRigidbody.AddExplosionForce(5f, source, 5f, 5f, ForceMode.Impulse);
    }

    public void BreakRope()
    {
        

    }

    private IEnumerator PlayParticlesOnHit(ContactPoint contactPoint, float forceHit)
    {
        GameObject instance = Instantiate(interactParticles);
        instance.transform.position = contactPoint.point;
        instance.transform.localScale = new Vector3(1, 1, 1);
        instance.transform.localScale *= Mathf.Lerp(0.3f, 1f, Mathf.InverseLerp(5, 30, forceHit));
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(particleSystem.main.duration);
        Destroy(instance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float force = collision.impulse.magnitude;

        if (force > 5f)
        {
            StartCoroutine(PlayParticlesOnHit(collision.contacts[0], force));
        }

        if (collision.gameObject.layer == 8)
        {

            if (connectedPin.createRope == null) { return; }
            ConnectedObject[] objects = connectedPin.createRope.GetConnectedObjects();
            int index = 0;
            if (connectedPin.indexConnect == 0) { index = 1; }
            else { index = 0; }

            if (collision.collider.gameObject.GetComponent<IRopeCollision>().GetUniqueID() == objects[index].uniqueID)
            {
                
                connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
            }
        }


    }

}
