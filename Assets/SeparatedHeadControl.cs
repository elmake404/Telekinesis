using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparatedHeadControl : MonoBehaviour
{
    [HideInInspector] public GameObject destroyHeadParticles;

    private void DestroyThisObject()
    {
        StartCoroutine(PlayDestroyHeadParicles());
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > 5f)
        {
            //connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
            Renderer renderer = gameObject.GetComponent<Renderer>();
            TemporaryRendererContainer.instance.DeleteRenderer(renderer.GetHashCode());
            DestroyThisObject();
        }

    }

    private IEnumerator PlayDestroyHeadParicles()
    {
        GameObject particles = Instantiate(destroyHeadParticles);
        particles.transform.position = transform.position;
        float duration = particles.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(particles);
        
    }
}
