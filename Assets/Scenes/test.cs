using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("impulse " + collision.impulse.magnitude);
        //Debug.Log("relative velocity " + collision.relativeVelocity);

    }


}
