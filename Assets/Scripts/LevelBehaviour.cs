using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelBehaviour : MonoBehaviour
{
    public Transform point;

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _survivalTransform;
    [SerializeField] private SurvivorCallbackReceiver survivorCallbackReceiver;
    [SerializeField] private SerializeStructPlatforms _serializeStructPlatforms;
    [SerializeField] private Animator _survivorAnimator;

    private PlatformsGenerator _platformsGenerator;
    private BoundsCameraUtility _boundsCameraUtility;
    private PlatformMoveBehaviour _platformMoveBehaviour;
    private SurvivorBehaviour _survivorBehaviour;
    private SurvivorAnimatorBehaviour _animatorBehaviour;

    public PlatformsGenerator platformsGenerator { get { return _platformsGenerator; } private set { } }
    public BoundsCameraUtility boundsCameraUtility { get { return _boundsCameraUtility; } private set{ } }
    public PlatformMoveBehaviour platformMoveBehaviour { get { return _platformMoveBehaviour; } private set { } }
    public SurvivorBehaviour survivorBehaviour { get { return _survivorBehaviour; } private set { } }
    public Transform survivalTransform { get { return _survivalTransform; } private set { } }

    public delegate void UpdaterDelegate();
    public UpdaterDelegate updater;
    public event UpdaterDelegate OnActionUpdater;

    void Start()
    {
        _boundsCameraUtility = new BoundsCameraUtility(_camera, this);
        _survivorBehaviour = new SurvivorBehaviour(survivorCallbackReceiver, this);
        _animatorBehaviour = new SurvivorAnimatorBehaviour(_survivorAnimator, _survivorBehaviour);
        _platformMoveBehaviour = new PlatformMoveBehaviour(this);
        PlatformsGenerator.DelegateCreateNewPlatform delegateCreateNewPlatform = _survivorBehaviour.SubscribeEnemiesWhenPlatformCreates;
        PlatformsGenerator.DelegateCreateLastPlatform delegateCreateLastPlatform = _survivorBehaviour.survivorMoveBehaviour.CalculateDistanceToTransformLastPlatform;
        _platformsGenerator = new PlatformsGenerator(_serializeStructPlatforms, this, delegateCreateNewPlatform, delegateCreateLastPlatform);
        
        
    }

    void Update()
    {
        OnActionUpdater?.Invoke();
    }

    public void SubscribeUpdaterDelegate(UpdaterDelegate updaterDelegate)
    {
        OnActionUpdater += updaterDelegate;
    }

    public void UnSubscribeUpdaterDelegate(UpdaterDelegate updaterDelegate)
    {
        OnActionUpdater -= updaterDelegate;
        //Debug.Log("Usubscribe");
    }
}
