using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRun : ZombieBehaviour
{
    private float minDistanceToTarget = 1.0f;

    protected override void OnEnable()
    {
        currentTask = ZombieTaskLog.targetSearch;    
    }

    protected override void FixedUpdate()
    {
        MoveToPlayer();
        RotateToPlayer();
        if (CalculateDistanceToCivillian() < minDistanceToTarget)
        {
            Debug.Log("Completed Task: " + currentTask);
            thisZombieBehaviour.CompletedTask(currentTask);
            
        }
    }

}
