using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorCallbackReceiver : MonoBehaviour
{
    public delegate void DelegateOnCollisionEnter(Collision collision);
    public event DelegateOnCollisionEnter OnActionCollisionEnter;

    public delegate void DelegateOnCollisionStay(Collision collision);
    public event DelegateOnCollisionStay OnActionCollisionStay;

    public delegate void DelegateOnCollisionExit(Collision collision);
    public event DelegateOnCollisionExit OnActionCollisionExit;

    private void OnCollisionEnter(Collision collision)
    {
        
        OnActionCollisionEnter?.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        OnActionCollisionStay?.Invoke(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        OnActionCollisionExit?.Invoke(collision);
    }


    public void SubscribeOnActionCollisionEnter(DelegateOnCollisionEnter delegateOnCollisionEnter)
    {
        OnActionCollisionEnter += delegateOnCollisionEnter;
    }

    public void UnSubscribeOnActionCollisionEnter(DelegateOnCollisionEnter delegateOnCollisionEnter)
    {
        OnActionCollisionEnter -= delegateOnCollisionEnter;
    }

    public void SubscribeOnActionCollisionStay(DelegateOnCollisionStay delegateOnCollisionStay)
    {
        OnActionCollisionStay += delegateOnCollisionStay;
    }

    public void UnSubscribeOnActionCollisionStay(DelegateOnCollisionStay delegateOnCollisionStay)
    {
        OnActionCollisionStay -= delegateOnCollisionStay;
    }

    public void SubscribeOnActionCollisionExit(DelegateOnCollisionExit delegateOnCollisionExit)
    {
        OnActionCollisionExit += delegateOnCollisionExit;
    }

    public void UnSubscribeOnActionCollisionExit(DelegateOnCollisionExit delegateOnCollisionExit)
    {
        OnActionCollisionExit -= delegateOnCollisionExit;
    }
}
