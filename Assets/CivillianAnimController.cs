using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CivillianAnimController : MonoBehaviour
{
    public Animator animator;

    private int idlePanic;
    private int idle;
    private int death;

    private CivillianAnimState animState = CivillianAnimState.panicRun;

    private void Start()
    {
        idlePanic = Animator.StringToHash("IdlePanic");
        idle = Animator.StringToHash("Idle");
        death = Animator.StringToHash("Death");
    }

    public void ChangeAnimState(CivillianAnimState state)
    {
        if (state == animState) { return; }

        switch (state)
        {
            case CivillianAnimState.panicIdle:
                animator.SetTrigger(idlePanic);
                break;
            case CivillianAnimState.idle:
                
                animator.SetTrigger(idle);
                break;
            case CivillianAnimState.death:
                animator.SetTrigger(death);
                break;
            
        }
    }
}
