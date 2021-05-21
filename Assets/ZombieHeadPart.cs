using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHeadPart : MonoBehaviour, IRopeCollision, IExploded
{
    public CurrentZombieControl zombieControl;
    public Collider headCollider;
    public Rigidbody headRigidbody;
    public CharacterJoint headJoint;
    public GameObject splatBloodParticles;
    public GameObject destroyedHeadParticles;
    public TypeOfConnected selectedType = TypeOfConnected.zombieHead;
    private bool isDetachHead = false;
    private float minImpulseToActive = 2f;

    public int GetUniqueID()
    {
        return zombieControl.gameObject.GetInstanceID();
    }

    public TypeOfConnected GetTypeOfConnected()
    {
        return selectedType;
    }

    public void Explode()
    {
        zombieControl.EnableRagdoll();
    }

    public void InitConnect()
    {
        zombieControl.isPinned = true;
    }

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        zombieControl.connectedPin = connectedPin;
    }

    public void BreakRope()
    {
        zombieControl.isRopeBreak = true;
        zombieControl.isPinned = false;
    }

    public void Explode(Vector3 source)
    {
        
        zombieControl.AddExplosionForceToBody(source);
        zombieControl.EnableRagdoll();
        ExplodeHeadOnExplode();
    }

    private Rigidbody SeparateHead()
    {
        GameObject head = zombieControl.GetHeadTransform().GetChild(0).gameObject;
        head.transform.SetParent(null);
        SeparatedHeadControl separatedHead = head.AddComponent<SeparatedHeadControl>();
        //separatedHead.SetConnectedPin(zombieControl.connectedPin);
        separatedHead.destroyHeadParticles = destroyedHeadParticles;
        SphereCollider sphereCollider = head.AddComponent<SphereCollider>();
        Rigidbody rigidbody = head.AddComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        sphereCollider.radius = 0.3f;
        return rigidbody;
    }

    private void DetachHead()
    {
        isDetachHead = true;
        
        headCollider.enabled = false;
        headRigidbody.isKinematic = false;
        headRigidbody.useGravity = true;
        SeparateHead();
        StartCoroutine(PlayAndDeleteBloodSplat());
        //zombieControl.connectedPin.createRope.ChangeConnectedObjectToPin(newObjRigidbody, zombieControl.connectedPin.indexConnect);
        zombieControl.StartRoutineDelayDeath();
    }

    
    private void ExplodeHeadOnExplode()
    {
        headCollider.enabled = false;
        headRigidbody.isKinematic = false;
        headRigidbody.useGravity = true;
        Transform headTransform = zombieControl.GetHeadTransform();

        int hash = headTransform.GetComponent<Renderer>().GetHashCode();
        TemporaryRendererContainer.instance.DeleteRenderer(hash);

        headTransform.GetChild(0).gameObject.SetActive(false);

        StartCoroutine(PlayDestroyHeadParicles(headTransform.position));
        StartCoroutine(PlayAndDeleteBloodSplat());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (zombieControl.isRopeBreak == true) { return; }

        float force = collision.impulse.magnitude;

        if (force > minImpulseToActive)
        {
            DetachHead();
            zombieControl.EnableRagdoll();
            StartCoroutine(PlayParticlesOnSimpleHead(collision.contacts[0].point, force));
            StartCoroutine(PlayDirectParticles(collision.contacts[0], force));
        }


        if (zombieControl.isPinned == true)
        {
            if (collision.gameObject.layer == 8)
            {
                ConnectedObject[] objects = zombieControl.connectedPin.createRope.GetConnectedObjects();
                int index = 0;
                if (zombieControl.connectedPin.indexConnect == 0) { index = 1; }
                else { index = 0; }


                if (collision.collider.gameObject.GetComponent<IRopeCollision>().GetUniqueID() == objects[index].uniqueID)
                {
                    zombieControl.connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
                }
            }
        }

    }

    private IEnumerator PlayParticlesOnSimpleHead(Vector3 pos, float forceHit)
    {
        GameObject instance = Instantiate(zombieControl.particlesOnHit);
        instance.transform.position = pos;
        instance.transform.localScale = new Vector3(1, 1, 1);
        instance.transform.localScale *= Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(5, 30, forceHit));
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(particleSystem.main.duration);
        Destroy(instance);
    }

    private IEnumerator PlayDirectParticles(ContactPoint contactPoint, float forceHit)
    {
        GameObject instance = Instantiate(zombieControl.directParticlesOnHit);
        instance.transform.position = contactPoint.point;
        instance.transform.localRotation = Quaternion.FromToRotation(Vector3.back, contactPoint.normal);
        instance.transform.localScale = new Vector3(1, 1, 1);
        instance.transform.localScale *= Mathf.Lerp(1f, 2f, Mathf.InverseLerp(5, 30, forceHit));
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(particleSystem.main.duration);
        Destroy(instance);
    }

    private IEnumerator PlayDestroyHeadParicles(Vector3 pos)
    {
        GameObject particles = Instantiate(destroyedHeadParticles);
        particles.transform.position = pos;
        float duration = particles.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(particles);
        yield return null;

    }

    private IEnumerator PlayAndDeleteBloodSplat()
    {
        GameObject particles = Instantiate(splatBloodParticles);
        particles.transform.position = transform.position;
        particles.transform.SetParent(transform);
        ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
        float duration = particleSystem.main.duration + 1f;
        yield return new WaitForSeconds(duration);
        Destroy(particles);
        yield return null;
    }
}
