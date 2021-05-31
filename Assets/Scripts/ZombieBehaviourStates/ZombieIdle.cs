using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdle : ZombieBehaviour
{
    protected override void OnEnable()
    {
        
    }

    protected override void Start()
    {
        currentBehaviourState = ZombieBehaviourState.zombieIdle;
    }
}
