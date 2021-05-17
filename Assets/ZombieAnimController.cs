using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieAnimationState
{
    isEmerge,
    isWalk,
    isAttack,
    isIdle,
    isEating,
    isAnimatorOff
}

public class ZombieAnimController : MonoBehaviour
{
    public Animator animator;
    private int walkID;
    private int attackID;
    private int idleID;
    private int eatingID;
    private ZombieAnimationState animationState = ZombieAnimationState.isEmerge;

    void Start()
    {
        walkID = Animator.StringToHash("isWalk");
        attackID = Animator.StringToHash("isAttack");
        idleID = Animator.StringToHash("isIdle");
        eatingID = Animator.StringToHash("isEating");
    }

    public void SetAnimation(ZombieAnimationState state)
    {
        if (state == ZombieAnimationState.isAnimatorOff) { return; }
        if (state == animationState) { return; }
        

        switch (state)
        {
            case ZombieAnimationState.isEmerge:
                break;
            case ZombieAnimationState.isAttack:
                animator.SetBool(attackID, true);
                break;
            case ZombieAnimationState.isWalk:
                animator.SetBool(walkID, true);
                break;
            case ZombieAnimationState.isIdle:
                animator.SetBool(idleID, true);
                break;
            case ZombieAnimationState.isEating:
                animator.SetBool(eatingID, true);
                break;
        }
    }

    public void DisableAnimator()
    {
        animator.enabled = false;
        animationState = ZombieAnimationState.isAnimatorOff;
    }

    
}
