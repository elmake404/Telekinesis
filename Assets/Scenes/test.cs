using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Collider collider1;
    public Collider collider2;

    private void Start()
    {
        Physics.IgnoreCollision(collider1, collider2);
    }
}

