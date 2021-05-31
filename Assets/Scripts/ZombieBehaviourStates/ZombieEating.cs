using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEating : ZombieBehaviour
{
    protected override void OnEnable()
    {
        
    }

    protected override void Start()
    {
        currentBehaviourState = ZombieBehaviourState.zombieEating;
    }
}
