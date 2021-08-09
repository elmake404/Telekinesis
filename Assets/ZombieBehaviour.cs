using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieState
{
    none,
    zombieRun,
    zombieAttack,
    zombieIdle,
    zombieEating,
    zombieDie
}


public class ZombieBehaviour : MonoBehaviour
{
    [SerializeField] private ZombieAnimController zombieAnimController;
    [SerializeField] private BlankZombie blankEnemy;
    public GameObject bloodParticles;

    [HideInInspector] public ConnectedPin connectedPin;
    [HideInInspector] public bool isInterCollisionWithOther = false;
    
    
    //[HideInInspector] protected SpawnZombies spawnZombies;
    [HideInInspector] public bool isRopeBreak = false;
    [HideInInspector] public bool isPinned = false;

    private ZombieState currentState = ZombieState.none;
    private Transform _survivorTransform;
    private Transform _platformTransform;
    private GameObject thisGameObject;
    private float yRotate = 0;
    private bool isEnabledRagdoll = false;
    private Rigidbody[] rigidbodies;

    public delegate void DelegateAttackAnimationTrigger();
    public event DelegateAttackAnimationTrigger OnAttackAnimationTrigger;

    private void Start()
    {
        SetIgnoreSelfCollisions();
        thisGameObject = gameObject;
        InitRigidBodies();
        SwichZombieState(ZombieState.zombieRun);
    }

    private void FixedUpdate()
    {
        IterateCurrentState();
    }

    public void AddExpectedObjectToCollision(int id)
    {
        blankEnemy.AddExpectedObjectToAllBody(id);
    }

    public void SubscribeDelegateAttackAnimationTrigger(DelegateAttackAnimationTrigger delegateAttackAnimationTrigger)
    {
        OnAttackAnimationTrigger += delegateAttackAnimationTrigger;
    }

    public void SwichZombieState(ZombieState state)
    {
        if (currentState == state) { return; }
        currentState = state;

        switch (state)
        {
            case ZombieState.none:
                break;
            case ZombieState.zombieRun:
                break;
            case ZombieState.zombieAttack:
                zombieAnimController.SetAnimation(ZombieAnimationState.isAttack);
                //spawnZombies.StopAnotherZombies(this.GetHashCode());
                break;
            case ZombieState.zombieIdle:
                zombieAnimController.SetAnimation(ZombieAnimationState.isIdle);
                break;
            case ZombieState.zombieEating:
                break;
            case ZombieState.zombieDie:
                zombieAnimController.DisableAnimator();
                break;
        }
    }

    private void IterateCurrentState()
    {
        switch (currentState)
        {
            case ZombieState.none:
                break;
            case ZombieState.zombieRun:
                MoveToPlayer();
                RotateToTarget();
                float distance = CalculateDistanceToCivillian();
                if (distance < 1.0f)
                {
                    SwichZombieState(ZombieState.zombieAttack);
                    this.enabled = false;
                }
                break;
        }
    }

    public void SetSurvivorTransform(Transform transform)
    {
        _survivorTransform = transform;
    }

    public void SetPlatformTransform(Transform transform)
    {
        _platformTransform = transform;
    }

    public void SetSpawnZombies(SpawnZombies spawn)
    {
        //spawnZombies = spawn;
    }

    public void AttackTriggerAnimation()
    {
        
        OnAttackAnimationTrigger?.Invoke();
        //Debug.Log("Attack!!");
    }
    
    private void SetIgnoreSelfCollisions()
    {
        Collider[] colliders = transform.GetComponentsInParent<Collider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            for (int k = 0; k < colliders.Length; k++)
            {
                if (i == k) { continue; }
                Physics.IgnoreCollision(colliders[i], colliders[k]);
            }
        }
    }


    private void InitRigidBodies()
    {
        rigidbodies = thisGameObject.GetComponentsInChildren<Rigidbody>();
    }

    private void EnableRigidbodies()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = false;
            rigidbodies[i].useGravity = true;
        }
    }

    public void AddExplosionForceToBody(Vector3 source)
    {

    }

    public void IgnoreRopeColliders(Collider[] colliders)
    {
    
    }

    public bool GetIsEnabledRagdoll()
    {
        return isEnabledRagdoll;
    }

    public void InitDeathZombie()
    {
        if (currentState == ZombieState.zombieDie) { return; }
        SwichZombieState(ZombieState.zombieDie);
        blankEnemy.StartDelayReductionBreakForce();
        //spawnZombies.AddNumOfDeadZombies();
        InitRagdoll();
    }

    public void InitZombieExplosion(Vector3 sourceExplosion)
    {
        InitDeathZombie();
        ApplyForceOnExplosion(sourceExplosion);
    }

    private void InitRagdoll()
    {
        if (isEnabledRagdoll == true) { return; }
        isEnabledRagdoll = true;
        EnableRigidbodies();
    }

    private IEnumerator RunDestroyCharacter(ZombieBodyPartID partID)
    {
        blankEnemy.SetPartIsBroken(partID);
        blankEnemy.DestroyConfigurableJointsOnBodyPart(partID);
        blankEnemy.DisableBrokenBodyPart(partID);
        BodyPartAndPutInObj partAndPutInObj = blankEnemy.GetPartAndPutInObj(partID);
        Transform[] instantiatedBones = blankEnemy.GetInstantiatedBones();
        Collider[] leftColliders = blankEnemy.DestroyUnusualCollidersAndReturnUsed(partID, instantiatedBones);
        blankEnemy.MakeIgnoreCollisionsWithOtherColliders(leftColliders);
        DestroyAttachedConfigurableJoints(instantiatedBones[0]);

        yield return new WaitForEndOfFrame();

        ZombieBodyControl[] zombieBodyControls = blankEnemy.GetZombieBodyControls(instantiatedBones);
        if (blankEnemy.PartIsAttachedToRope(partAndPutInObj) == true)
        {
            int[] indexesInHirachly = blankEnemy.GetBonesIndexesFromHirachly(partAndPutInObj);
            Transform[] comparableBones = blankEnemy.GetBonesFromHirachlyRootBone(instantiatedBones[0], indexesInHirachly);
            List<List<ConnectedRope>> connectedRopes = blankEnemy.GetConnectedsRopesFromBodyPart(partAndPutInObj);
            ReconnectBodyPartsToRopes(comparableBones, connectedRopes);
        }

        blankEnemy.PrepareBonesForSlicedPart(zombieBodyControls, partID);
        Transform newRootBone;
        blankEnemy.ReBuildHirachlyBones(_platformTransform ,instantiatedBones[0], zombieBodyControls, partID, out newRootBone);
        SpawnParticles.InParent(this, bloodParticles, newRootBone);
        RelinkSlicedParts(instantiatedBones, partAndPutInObj);
        DisableZombieBodyPartsControllers(partAndPutInObj);

        yield return null;
    }


    private void ApplyForceOnExplosion(Vector3 sourceExplosion)
    {
        ZombieBodyPartID[] notYetDestroyedParts = blankEnemy.GetAllNotYetDeatroyedPartIDs();

        for (int i = 0; i < notYetDestroyedParts.Length; i++)
        {
            BodyPartAndPutInObj part = blankEnemy.GetPartAndPutInObj(notYetDestroyedParts[i]);

            for (int x = 0; x < part.zombieBodyControls.Length; x++)
            {
                part.zombieBodyControls[x].attachedRigidbody.AddExplosionForce(5f, sourceExplosion, 10f, 10f, ForceMode.Impulse);
            }
        }
    }

    private void DestroyAttachedConfigurableJoints(Transform rootBone)
    {
        ConfigurableJoint[] configurableJoints = rootBone.GetComponentsInChildren<ConfigurableJoint>();
        for (int i = 0; i < configurableJoints.Length; i++)
        {
            Destroy(configurableJoints[i]);
        }
    }

    private void ReconnectBodyPartsToRopes(Transform[] comparableBones, List<List<ConnectedRope>> connectedRopes)
    {
        for (int i = 0; i < comparableBones.Length; i++)
        {
            Rigidbody boneRigidbody = comparableBones[i].GetComponent<Rigidbody>();
            List<ConnectedRope> connectedRopesOnBone = connectedRopes[i];

            for (int k = 0; k < connectedRopesOnBone.Count; k++)
            {
                int orderInRope = connectedRopesOnBone[k].orderInRope;
                connectedRopesOnBone[k].attachedRopeBehaviour.SetNewRigidbodyToSelectedJoint(orderInRope, boneRigidbody);
            }
        }
    }

    public void InitBreakJoint(ZombieBodyPartID partID, ZombieBodyControl thatCall)
    {
        if (blankEnemy.PartIsBroken(partID) == true)
        {
            return;
        }

        StartCoroutine(RunDestroyCharacter(partID));
    }

    

    private void RelinkSlicedParts(Transform[] bones, BodyPartAndPutInObj bodyPartAndPut)
    {
        for (int i = 0; i < bodyPartAndPut.usedBodyParts.Length; i++)
        {
            bodyPartAndPut.usedBodyParts[i].transform.SetParent(null);
            SkinnedMeshRenderer meshRenderer = bodyPartAndPut.usedBodyParts[i].GetComponent<SkinnedMeshRenderer>();
            meshRenderer.rootBone = bones[0];
            meshRenderer.bones = bones;
            
        }
    }

    private void DisableZombieBodyPartsControllers(BodyPartAndPutInObj partAndPutInObj)
    {
        for (int i = 0; i < partAndPutInObj.zombieBodyControls.Length; i++)
        {
            partAndPutInObj.zombieBodyControls[i].enabled = false;
        }
    }

    private void RotateToTarget()
    {
        Vector3 normaDirToPlayer = (_survivorTransform.position - transform.position).normalized;
        Quaternion rotate = Quaternion.identity;
        Quaternion orignRotate = Quaternion.LookRotation(transform.forward);
        rotate = Quaternion.RotateTowards(orignRotate, rotate, Time.deltaTime);
        rotate.SetLookRotation(normaDirToPlayer);

        /*float yTargetRotate = Mathf.Atan2(normaDirToPlayer.x, transform.forward.z) * Mathf.Rad2Deg;
        yRotate = Mathf.MoveTowards(yRotate, yTargetRotate, 20f * Time.deltaTime);*/
        //transform.rotation = Quaternion.Euler(0, yRotate, 0);
        transform.rotation = rotate;
    }

    private void MoveToPlayer()
    {
        if (isInterCollisionWithOther == true) { return; }
        Vector3 playerPos = _survivorTransform.position;
        playerPos.y = 0f;
        Vector3 pos = Vector3.MoveTowards(transform.position, playerPos, Time.deltaTime * 0.5f);
        transform.position = pos;
    }

    private float CalculateDistanceToCivillian()
    {
        return Vector3.Distance(transform.position, _survivorTransform.position);
    }

}

public enum ZombieBodyPartID
{
    body,
    head,
    rightHand,
    leftHand,
    rightFoot,
    leftFoot
}



[System.Serializable]
public struct BodyPartAndPutInObj
{
    public ZombieBodyPartID zombieBodyPartID;
    public ZombieBodyControl[] zombieBodyControls;
    public GameObject[] usedBodyParts;
}