using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieControllerState
{
    zombieRun,
    zombieAttack,
    zombieIdle,
    zombieEating,
    zombieDie
}

public class CurrentZombieControl : MonoBehaviour
{
    public ZombieAnimController zombieAnimController;
    public BlankZombie blankEnemy;
    public ConnectedPin connectedPin;
    public GameObject particlesOnHit;
    public GameObject directParticlesOnHit;
    [HideInInspector] public bool isInterCollisionWithOther = false;
    [HideInInspector] public CivilianController civillianController;
    [HideInInspector] public SpawnZombies spawnZombies;
    [HideInInspector] public bool isRopeBreak = false;
    [HideInInspector] public bool isPinned = false;
    private Transform thisTransform;
    private GameObject thisGameObject;
    private float yRotate = 0;
    private float minDistanceToPlayer = 1f;
    private bool isEnabledRagdoll = false;
    private ZombieControllerState controllerState = ZombieControllerState.zombieRun;
    private Collider[] colliders;
    private Rigidbody[] rigidbodies;

    private void OnEnable()
    {
        thisTransform = transform;
        thisGameObject = gameObject;
    }

    private void Start()
    {
        InitColliders();
        InitRigidBodies();
    }

    private void FixedUpdate()
    {
        //SwitchStateEnemy(controllerState);
    }

    public void AttackTriggerAnimation()
    {
        spawnZombies.InitCivillianDead();
        controllerState = ZombieControllerState.zombieEating;
        SwitchStateEnemy(controllerState);
    }

    private void SwitchStateEnemy(ZombieControllerState state)
    {
        switch (state)
        {
            case ZombieControllerState.zombieRun:
                MoveToPlayer();
                RotateZombie();
                WaitToAttackState();
                break;
            case ZombieControllerState.zombieAttack:
                RotateZombie();
                zombieAnimController.SetAnimation(ZombieAnimationState.isAttack);
                this.enabled = false;
                break;
            case ZombieControllerState.zombieDie:
                this.enabled = false;
                break;
            case ZombieControllerState.zombieIdle:
                zombieAnimController.SetAnimation(ZombieAnimationState.isIdle);
                this.enabled = false;
                break;
            case ZombieControllerState.zombieEating:
                zombieAnimController.SetAnimation(ZombieAnimationState.isEating);
                break;
        }
    }

    private void InitColliders()
    {
        colliders = thisTransform.GetComponentsInChildren<Collider>();
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

    public void MakeIdleZombie()
    {
        controllerState = ZombieControllerState.zombieIdle;
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

    public void InitRagdoll()
    {
        if (isEnabledRagdoll == true) { return; }
        isEnabledRagdoll = true;
        zombieAnimController.DisableAnimator();
        EnableRigidbodies();
        
    }

    private IEnumerator RunDestroyCharacter(ZombieBodyPartID partID)
    {
        blankEnemy.SetPartIsBroken(partID);

        BodyPartAndPutInObj partAndPutInObj = blankEnemy.GetPartAndPutInObj(partID);
        Transform[] instantiatedBones = blankEnemy.GetInstantiatedBones();
        yield return new WaitForEndOfFrame();

        ZombieBodyControl[] zombieBodyControls = blankEnemy.GetZombieBodyControls(instantiatedBones);
        blankEnemy.PrepareBonesForSlicedPart(zombieBodyControls, partID);
        blankEnemy.ReBuildHirachlyBones(instantiatedBones[0], zombieBodyControls, partID);
        DisableZombieBodyPartsControllers(partAndPutInObj);

        RelinkSlicedParts(instantiatedBones, partAndPutInObj);

        blankEnemy.DisableBrokenBodyPart(partID);

        yield return null;
    }

    public void InitBreakJoint(ZombieBodyPartID partID)
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

    private void RotateZombie()
    {
        Vector3 playerCamPos = civillianController.transform.position;
        Vector3 normaDirToPlayer = (playerCamPos - transform.position).normalized;
        float yTargetRotate = Mathf.Atan2(normaDirToPlayer.x, transform.forward.z) * Mathf.Rad2Deg;
        yRotate = Mathf.MoveTowards(yRotate, yTargetRotate, 8f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, yRotate, 0);
    }
    private void MoveToPlayer()
    {
        if (isInterCollisionWithOther == true) { return; }
        Vector3 playerPos = civillianController.transform.position;
        playerPos.y = 0f;
        Vector3 pos = Vector3.MoveTowards(transform.position, playerPos, Time.deltaTime/3f);
        transform.position = pos;
    }

    private float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, civillianController.transform.position);
    }

    private void WaitToAttackState()
    {
        if (CalculateDistanceToPlayer() < minDistanceToPlayer)
        {
            controllerState = ZombieControllerState.zombieAttack;
            spawnZombies.StopAnotherZombies(this.GetHashCode());
        }
    }

    
    
    
}
