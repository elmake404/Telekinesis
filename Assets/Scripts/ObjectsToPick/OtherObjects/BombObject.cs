﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObject : ObjectToPick
{
    public Rigidbody thisRigidbody;
    public Collider thisCollider;
    public Transform parent;
    public GameObject particlesExplosion;
    private ConnectedPin connectedPin;
    private float minImpulseStrength = 5f;
    private float radiusExplosion = 3f;
    private bool isExplode = false;

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
        thisCollider.enabled = true;
        thisRigidbody.useGravity = true;
        thisRigidbody.isKinematic = false;
        
    }

    public override TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public override void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (isExplode == true) { return; }

        if (Mathf.Abs(collision.impulse.magnitude) > minImpulseStrength)
        {
            isExplode = true;
            MakeExplosion();
            StartCoroutine(PlayParticlesExplosion());
            DestroyThisObject();
        }
    }

    private void MakeExplosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusExplosion, 1 << 8);
        if (colliders.Length == 0) { return; }


        for (int i = 0; i < colliders.Length; i++)
        {
            IRopeCollision ropeCollision = colliders[i].gameObject.GetComponent<IRopeCollision>();
            if (ropeCollision.GetTypeOfConnected() == TypeOfConnected.barrelBomb) { continue; }

            IExploded exploded = colliders[i].gameObject.GetComponent<IExploded>();
            exploded.Explode(transform.position);

            ObjectToPick objectToPick = colliders[i].gameObject.GetComponent<ObjectToPick>();
            switch (objectToPick.selectedType)
            {
                case TypeOfConnected.zombieBody:
                case TypeOfConnected.zombieHead:
                case TypeOfConnected.zombieFoot:
                case TypeOfConnected.zombieHand:
                    ZombieBodyControl zombieBodyControl = objectToPick as ZombieBodyControl;
                    zombieBodyControl.zombieControl.InitZombieExplosion(transform.position);
                    break;
            }
        }
    }

    private IEnumerator PlayParticlesExplosion()
    {
        GameObject particles = Instantiate(particlesExplosion);
        particles.transform.position = transform.position;
        float duration = particles.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(particles);
    }

    /*private void ManualDestroyRope()
    {
        if (connectedPin.createRope == null) { return; }
        connectedPin.createRope.BreakRope();
    }*/

    private void DestroyThisObject()
    {
        int hash = gameObject.GetComponent<Renderer>().GetHashCode();
        TemporaryRendererContainer.instance.DeleteRenderer(hash);
        Destroy(parent.gameObject);
    }


}
