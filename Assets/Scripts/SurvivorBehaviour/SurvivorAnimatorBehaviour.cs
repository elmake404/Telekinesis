using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorAnimatorBehaviour
{
    private Animator _animator;
    private SurvivorBehaviour _survivorBehaviour;
    private string _triggerDeath = "Death";
    private string _triggerVictory = "Victory";
    private int _triggerDeathID;
    private int _triggerVictoryID;

    public SurvivorAnimatorBehaviour(Animator animator, SurvivorBehaviour survivorBehaviour)
    {
        _animator = animator;
        _survivorBehaviour = survivorBehaviour;
        _triggerDeathID = Animator.StringToHash(_triggerDeath);
        _triggerVictoryID = Animator.StringToHash(_triggerVictory);
        _survivorBehaviour.SubscribeDelegateDeathSurvivor(ActivateDeathAnimation);
        _survivorBehaviour.survivorMoveBehaviour.SubscribeDelegateSurvivorArriveToLastPlatform(ActivateVictoryAnimation);
    }

    private void ActivateDeathAnimation()
    {
        _animator.SetTrigger(_triggerDeathID);
    }

    private void ActivateVictoryAnimation()
    {
        _animator.SetTrigger(_triggerVictoryID);
    }
}
