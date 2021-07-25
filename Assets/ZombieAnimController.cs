using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieAnimationState
{
    isWalk,
    isAttack,
    isIdle,
    isEating,
    isAnimatorOff
}

public class ZombieAnimController : MonoBehaviour
{
    public Animator animator;
    private int attackID;
    private int idleID;
    private ZombieAnimationState animationState = ZombieAnimationState.isWalk;

    void Start()
    {
        attackID = Animator.StringToHash("attack");
        idleID = Animator.StringToHash("idle");
    }

    public void SetAnimation(ZombieAnimationState state)
    {
        if (state == ZombieAnimationState.isAnimatorOff) { return; }
        if (state == animationState) { return; }

        switch (state)
        {
            case ZombieAnimationState.isAttack:
                animator.SetTrigger(attackID);
                //Debug.Log("Attack anim state");
                break;
            case ZombieAnimationState.isIdle:
                animator.SetTrigger(idleID);
                break;
        }
    }

    public void DisableAnimator()
    {
        animator.enabled = false;
        animationState = ZombieAnimationState.isAnimatorOff;
    }

    
}
