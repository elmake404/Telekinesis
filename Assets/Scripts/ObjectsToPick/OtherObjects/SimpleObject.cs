using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleObject : ObjectToPick
{
    public Rigidbody objectRigidbody;
    public Collider objectCollider;
    public GameObject interactParticles;
    private ConnectedPin connectedPin;

    protected override void OnEnable()
    {
        connectedRopes = new List<ConnectedRope>();
    }

    protected override void Start()
    {
        InitEnable();
    }

    public override int GetUniqueID()
    {
        return gameObject.GetInstanceID();
    }

    public override void InitComponent()
    {
        this.enabled = true;
    }

    private void InitEnable()
    {
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        objectCollider.enabled = true;
    }

    public override void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    public override TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
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

    protected override void OnCollisionEnter(Collision collision)
    {
        float force = collision.impulse.magnitude;

        if (force > 5f)
        {
            StartCoroutine(PlayParticlesOnHit(collision.contacts[0], force));
        }

        

    }

}
