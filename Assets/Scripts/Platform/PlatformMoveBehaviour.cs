using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoveBehaviour
{
    public delegate void DelegateMove(float speedMove);
    public event DelegateMove OnActionMove;

    private LevelBehaviour _levelBehaviour;

    public PlatformMoveBehaviour(LevelBehaviour levelBehaviour)
    {
        _levelBehaviour = levelBehaviour;
        _levelBehaviour.SubscribeUpdaterDelegate(UpdateMove);
    }

    public void SubscribePlatformToMove(DelegateMove delegateMove)
    {
        OnActionMove += delegateMove;
    }

    public void UnSubscribeFromPlatformMove(DelegateMove delegateMove)
    {
        OnActionMove -= delegateMove;
    }

    private void UpdateMove()
    {
        OnActionMove?.Invoke(_levelBehaviour.survivorBehaviour.survivorMoveBehaviour.SpeedMove);
    }
}
