using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToPick : MonoBehaviour, IRopeCollision, IExploded, IInitObject
{
    public TypeOfConnected selectedType = TypeOfConnected.none;
    //[HideInInspector] public RopeBehaviour currentRopeBehaviour;
    //[HideInInspector] public int ownOrderInRope = 1;
    [HideInInspector] public bool isAttachedToRope = false;
    protected List<ConnectedRope> connectedRopes;

    public virtual TypeOfConnected GetTypeOfConnected()
    {
        return TypeOfConnected.none;
    }

    public virtual int GetUniqueID()
    {
        return -1;
    }

    public virtual void InitConnect()
    {
        
    }

    public virtual void BreakRope()
    {
        
    }

    public virtual void SetWithRopeConnected(ConnectedPin connectedPin)
    {
        
    }
    // Explode Interface
    public virtual void Explode(Vector3 source)
    {
        
    }
    // InitObject Interface
    public virtual void InitComponent()
    {

    }

    public virtual void DisableComponent()
    {
        this.enabled = false;
    }

    public virtual void AddConnectedRope(ConnectedRope connectedRope)
    {
        connectedRopes.Add(connectedRope);   
    }

    public List<ConnectedRope> GetConnectedRopes()
    {
        return connectedRopes;
    }

    protected virtual void OnEnable() { }
    protected virtual void Start() { }
    protected virtual void OnCollisionEnter(Collision collision){ }

    protected virtual void OnJointBreak(float breakForce) { }
    protected virtual void OnDisable() { }

    protected virtual float GetRelativeVelocityMagnitude(Collision collision)
    {
        return collision.relativeVelocity.magnitude;
    }
}
