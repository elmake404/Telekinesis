using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorMoveBehaviour
{
    private float _speedMove = 1f;
    private float _maxSpeedMove = 1f;
    private float _minSpeedAtDeceleration = 0.3f;
    private LevelBehaviour _levelBehaviour;
    private SurvivorBehaviour _survivorBehaviour;
    private Transform _survivorTransform;
    private PlatformBehaviour _lastPlatformBehaviour;
    private Transform _lastPlatformTransform;

    public delegate void DelegateStartDeceleration();
    public event DelegateStartDeceleration OnStartDeceleration;

    public delegate void DelegateStopDeceleration();
    public event DelegateStopDeceleration OnStopDeceleration;

    public delegate void DelegateSurvivorArriveToLastPlatform();
    public event DelegateSurvivorArriveToLastPlatform OnArriveToLastPlatform;

    public float SpeedMove { get { return _speedMove; } private set { } }


    public SurvivorMoveBehaviour(SurvivorBehaviour survivorBehaviour)
    {
        _levelBehaviour = survivorBehaviour.levelBehaviour;
        _survivorBehaviour = survivorBehaviour;

        _survivorBehaviour.SubscribeDelegateDeathSurvivor(delegate
        {
            _levelBehaviour.UnSubscribeUpdaterDelegate(SurvivorArriveToTransformLastPlatform);
            _survivorBehaviour.survivorCallbackReceiver.UnSubscribeOnActionCollisionEnter(OnCollisionEnter);
            _survivorBehaviour.survivorCallbackReceiver.UnSubscribeOnActionCollisionEnter(OnCollisionExit);
            OnStartDeceleration -= StartDeceleration;
            OnStopDeceleration -= StopDeceleration;
            _levelBehaviour.UnSubscribeUpdaterDelegate(DecelerationSpeed);
            StopMove();
        });

        _survivorBehaviour.survivorCallbackReceiver.SubscribeOnActionCollisionEnter(OnCollisionEnter);
        _survivorTransform = _survivorBehaviour.survivorCallbackReceiver.transform;
        //_survivorBehaviour.survivorCallbackReceiver.SubscribeOnActionCollisionStay(OnCollisionStay);
        _survivorBehaviour.survivorCallbackReceiver.SubscribeOnActionCollisionExit(OnCollisionExit);

        OnArriveToLastPlatform += delegate 
        {
            _levelBehaviour.UnSubscribeUpdaterDelegate(SurvivorArriveToTransformLastPlatform);
            _survivorBehaviour.survivorCallbackReceiver.UnSubscribeOnActionCollisionEnter(OnCollisionEnter);
            _survivorBehaviour.survivorCallbackReceiver.UnSubscribeOnActionCollisionEnter(OnCollisionExit);
            OnStartDeceleration -= StartDeceleration;
            OnStopDeceleration -= StopDeceleration;
            _levelBehaviour.UnSubscribeUpdaterDelegate(DecelerationSpeed);
            StopMove();
        };
        
        OnStartDeceleration += StartDeceleration;
        OnStopDeceleration += StopDeceleration;
    }

    public void SubscribeDelegateSurvivorArriveToLastPlatform(DelegateSurvivorArriveToLastPlatform delegateSurvivorArriveToLastPlatform)
    {
        OnArriveToLastPlatform += delegateSurvivorArriveToLastPlatform;
    }

    public void UnSubscribeDelegateSurvivorArriveToLastPlatform(DelegateSurvivorArriveToLastPlatform delegateSurvivorArriveToLastPlatform)
    {
        OnArriveToLastPlatform -= delegateSurvivorArriveToLastPlatform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = new ContactPoint[5];
        int numOfContacts = collision.GetContacts(contactPoints);

        for (int i = 0; i < numOfContacts; i++)
        {
            if (contactPoints[i].otherCollider.gameObject.layer == 8)
            {
                TypeOfConnected type = contactPoints[i].otherCollider.gameObject.GetComponent<IRopeCollision>().GetTypeOfConnected();
                if (type == TypeOfConnected.simpleObject)
                {
                    OnStartDeceleration?.Invoke();
                    break;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        OnStopDeceleration?.Invoke();
    }

    

    private void StartDeceleration()
    {
        _levelBehaviour.SubscribeUpdaterDelegate(DecelerationSpeed);
    }

    private void StopDeceleration()
    {
        ///Debug.Log("UnsubscribeDeceleration");
        _levelBehaviour.UnSubscribeUpdaterDelegate(DecelerationSpeed);
        
        _speedMove = _maxSpeedMove;
    }

    private void DecelerationSpeed()
    {
        _speedMove = Mathf.Lerp(_speedMove, _minSpeedAtDeceleration, 2f * Time.deltaTime);
        //Debug.Log("Deceleration");
    }



    private void StopMove()
    {
        _speedMove = 0f;
    }

    public void CalculateDistanceToTransformLastPlatform(PlatformBehaviour platformBehaviour)
    {
        _lastPlatformBehaviour = platformBehaviour;
        _lastPlatformTransform = platformBehaviour.transform;
        _levelBehaviour.SubscribeUpdaterDelegate(SurvivorArriveToTransformLastPlatform);
        Debug.Log("CreateLastPlatform  " + platformBehaviour.transform.position);
    }


    private void SurvivorArriveToTransformLastPlatform()
    {
        float dist = Vector3.Distance(_survivorTransform.position, _lastPlatformTransform.position);
        if (dist < 0.1f)
        {
            OnArriveToLastPlatform?.Invoke();

        }
        
    }
}
