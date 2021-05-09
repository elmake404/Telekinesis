using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparatedHeadControl : MonoBehaviour
{
    private ConnectedPin connectedPin;
    [HideInInspector] public GameObject destroyHeadParticles;
    [HideInInspector] public Rigidbody rigidbody;
    

    public void SetConnectedPin(ConnectedPin connectedPin)
    {
        this.connectedPin = connectedPin;
        
    }

    private void DestroyThisObject()
    {
        StartCoroutine(PlayDestroyHeadParicles());
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > 5f)
        {
            connectedPin.createRope.ManualBreakRopeIfConnectedObjCollided();
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

    private IEnumerator PlayDestroyHeadParicles()
    {
        GameObject particles = Instantiate(destroyHeadParticles);
        particles.transform.position = transform.position;
        float duration = particles.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(particles);
        
    }
}
