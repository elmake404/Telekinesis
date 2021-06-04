using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Rigidbody boxRigidbody;

    private void Start()
    {
        boxRigidbody.maxDepenetrationVelocity = 0.1f;
    }
}

