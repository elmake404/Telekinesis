using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorBehaviour
{
    private SurvivorMoveBehaviour _survivorMoveBehaviour;
    private SurvivorCallbackReceiver _survivorCallbackReceiver;
    private LevelBehaviour _levelBehaviour;

    public SurvivorMoveBehaviour survivorMoveBehaviour { get { return _survivorMoveBehaviour; } private set { } }
    public SurvivorCallbackReceiver survivorCallbackReceiver { get { return _survivorCallbackReceiver; } private set { } }
    public LevelBehaviour levelBehaviour { get { return _levelBehaviour; } private set { } }

    public delegate void DelegateSurvivorDeath();
    public event DelegateSurvivorDeath OnSurvivorDeath;

    public SurvivorBehaviour(SurvivorCallbackReceiver survivorCallbackReceiver, LevelBehaviour levelBehaviour)
    {
        _survivorCallbackReceiver = survivorCallbackReceiver;
        _levelBehaviour = levelBehaviour;
        _survivorMoveBehaviour = new SurvivorMoveBehaviour(this);
       
    }

    private void SurvivorDeath()
    {
        OnSurvivorDeath?.Invoke();
    }

    public void SubscribeDelegateDeathSurvivor(DelegateSurvivorDeath delegateSurvivorDeath)
    {
        OnSurvivorDeath += delegateSurvivorDeath;
    }

    public void UnSubscribeDelegateDeathSurvivor(DelegateSurvivorDeath delegateSurvivorDeath)
    {
        OnSurvivorDeath -= delegateSurvivorDeath;
    }

    public void SubscribeEnemiesWhenPlatformCreates(PlatformBehaviour platformBehaviour)
    {
        PlatformType platformType = platformBehaviour.platformType.platformType;
        switch (platformType)
        {
            case PlatformType.standartPlatformType:
                StandartPlatformBehaviour standartPlatformBehaviour = platformBehaviour as StandartPlatformBehaviour;
                standartPlatformBehaviour.SubscribeDelegateCreateNewZombie(SubscribeAttacActionWhenEnemyCreated);
                break;
        }
    }

    private void SubscribeAttacActionWhenEnemyCreated(ZombieBehaviour behaviour)
    {
        behaviour.SubscribeDelegateAttackAnimationTrigger(SurvivorDeath);
    }
}
