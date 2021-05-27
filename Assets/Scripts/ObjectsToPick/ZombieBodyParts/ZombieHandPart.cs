using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHandPart : ZombieBodyControl
{
    

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void OnJointBreak(float breakForce)
    {
        base.OnJointBreak(breakForce);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        
    }
}
