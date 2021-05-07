using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHeadPart : MonoBehaviour, IRopeCollision
{
    public CurrentZombieControl zombieControl;
    public Collider headCollider;
    public Rigidbody headRigidbody;
    public CharacterJoint headJoint;
    public GameObject splatBloodParticles;
    private ConnectedPin connectedPin;


    public void InitConnect()
    {
        headCollider.enabled = false;
        headRigidbody.isKinematic = false;
        headRigidbody.useGravity = true;
        //headJoint.connectedBody = null;
        Rigidbody newObjRigidbody = SeparateHead();
        StartCoroutine(PlayAndDeleteBloodSplat());
        connectedPin.createRope.ChangeConnectedObjectToPin(newObjRigidbody, connectedPin.indexConnect);
        zombieControl.StartRoutineDelayDeath();
    }

    public void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
    }

    public void BreakRope(Vector3 source)
    {
        return;
    }

    private Rigidbody SeparateHead()
    {
        GameObject head = zombieControl.GetHeadTransform().GetChild(0).gameObject;
        head.transform.SetParent(null);
        SphereCollider sphereCollider = head.AddComponent<SphereCollider>();
        Rigidbody rigidbody = head.AddComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        sphereCollider.radius = 0.3f;
        return rigidbody;
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
