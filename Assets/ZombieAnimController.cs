using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieAnimationState
{
    isEmerge,
    isWalk,
    isAttack,
    isAnimatorOff
}

public class ZombieAnimController : MonoBehaviour
{
    public Animator animator;
    private int walkID;
    private int attackID;
    private ZombieAnimationState animationState = ZombieAnimationState.isEmerge;

    void Start()
    {
        walkID = Animator.StringToHash("isWalk");
        attackID = Animator.StringToHash("isAttack");

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
            
        }
    }

    public void DisableAnimator()
    {
        animator.enabled = false;
        animationState = ZombieAnimationState.isAnimatorOff;
    }

    
}
