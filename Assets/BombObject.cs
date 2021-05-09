using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObject : MonoBehaviour, IRopeCollision
{
    public Transform parent;
    public GameObject particlesExplosion;
    private ConnectedPin connectedPin;
    public TypeOfConnected selectedType = TypeOfConnected.barrelBomb;
    private float minImpulseStrength = 5f;
    private float radiusExplosion = 20f;

    void Start()
    {

    }

    public void InitConnect()
    {

    }

    public void BreakRope(Vector3 source)
    {

    }

    public TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > minImpulseStrength)
        {
            MakeExplosion();
            StartCoroutine(PlayParticlesExplosion());
            ManualDestroyRope();
            DestroyThisObject();
        }

        else 
        {
            if (collision.gameObject.layer == 8)
            {
                if (connectedPin.createRope == null) { return; }
                ConnectedObject[] objects = connectedPin.createRope.GetConnectedObjects();
                int index = 0;
                if (connectedPin.indexConnect == 0) { index = 1; }
                else { index = 0; }

                if (collision.collider.gameObject.GetHashCode() == objects[index].attacheRigidbody.gameObject.GetHashCode())
                {
                    connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
                }
            }
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

    private void ManualDestroyRope()
    {
        if (connectedPin.createRope == null) { return; }
        connectedPin.createRope.BreakRope();
        
    }

    private void DestroyThisObject()
    {
        Destroy(parent.gameObject);
    }
}
