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
    private ConnectedPin connectedPin;
    private float minImpulseToActive = 5f;

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
        DetachHead();
    }

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    public void BreakRope(Vector3 source)
    {
        return;
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
        separatedHead.SetConnectedPin(connectedPin);
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
        headCollider.enabled = false;
        headRigidbody.isKinematic = false;
        headRigidbody.useGravity = true;
        Rigidbody newObjRigidbody = SeparateHead();
        StartCoroutine(PlayAndDeleteBloodSplat());
        connectedPin.createRope.ChangeConnectedObjectToPin(newObjRigidbody, connectedPin.indexConnect);
        zombieControl.StartRoutineDelayDeath();
    }

    private void ExplodeHeadOnExplode()
    {
        headCollider.enabled = false;
        headRigidbody.isKinematic = false;
        headRigidbody.useGravity = true;
        Transform headTransform = zombieControl.GetHeadTransform();
        headTransform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(PlayDestroyHeadParicles(headTransform.position));
        StartCoroutine(PlayAndDeleteBloodSplat());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > minImpulseToActive)
        {
            zombieControl.EnableRagdoll();
        }

    }

    private IEnumerator PlayDestroyHeadParicles(Vector3 pos)
    {
        GameObject particles = Instantiate(destroyedHeadParticles);
        particles.transform.position = pos;
        float duration = particles.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(particles);

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
