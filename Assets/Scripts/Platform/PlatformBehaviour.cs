using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlatformBehaviour : MonoBehaviour
{
    //public PlatformType platformType;
    public Platform platformType;

    protected Bounds _bounds;
    protected List<ZombieBehaviour> _zombieBehaviours;
    protected PlatformMoveBehaviour _platformMoveBehaviour;
    protected LevelBehaviour _levelBehaviour;
    protected Transform _survivalTransform;

    [SerializeField] protected MeshRenderer _rendererStand;


    public MeshRenderer RendererStand { get { return _rendererStand; } private set { } }

    protected virtual void Start()
    {
        _bounds = _rendererStand.bounds;
        _platformMoveBehaviour.SubscribePlatformToMove(MovePlatform);
    }

    protected virtual private void Update()
    {
        
    }

    protected virtual void MovePlatform(float speed)
    {
        transform.position += speed * Time.deltaTime * Vector3.back;
    }



    public virtual void ActivatePlatform()
    {

    }

    public void InitionalAssign(LevelBehaviour levelBehaviour)
    {
        this._levelBehaviour = levelBehaviour;
        _platformMoveBehaviour = levelBehaviour.platformMoveBehaviour;
        _platformMoveBehaviour.SubscribePlatformToMove(MovePlatform);
        _survivalTransform = levelBehaviour.survivalTransform;
    }
    
}
