using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieBehaviourState
{
    none,
    zombieRun,
    zombieAttack,
    zombieIdle,
    zombieEating,
    zombieDie
}

public enum ZombieTaskLog
{
    none,
    targetSearch,
    killCivillian
}

public class ZombieBehaviour : MonoBehaviour
{
    
    [SerializeField] private ZombieBehaviourStates zombieBehaviourStates;
    [SerializeField] private ZombieAnimController zombieAnimController;
    [SerializeField] private BlankZombie blankEnemy;

    [HideInInspector] public ConnectedPin connectedPin;
    [HideInInspector] public bool isInterCollisionWithOther = false;
    
    [HideInInspector] public SpawnZombies spawnZombies;
    [HideInInspector] public bool isRopeBreak = false;
    [HideInInspector] public bool isPinned = false;

    protected ZombieBehaviourState currentBehaviourState = ZombieBehaviourState.none;
    protected ZombieTaskLog currentTask = ZombieTaskLog.none;
    protected ZombieBehaviour thisZombieBehaviour;
    protected CivilianController civillianController;
    private bool[] zombieTasksIsCompleted = new bool[Enum.GetNames(typeof(ZombieTaskLog)).Length];
    private Transform thisTransform;
    private GameObject thisGameObject;
    private float yRotate = 0;
    private float minDistanceToPlayer = 1f;
    private bool isEnabledRagdoll = false;
    private Rigidbody[] rigidbodies;
    public HashSet<int> hashCodeColliders;

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void Start()
    {
        thisTransform = transform;
        thisGameObject = gameObject;
        thisZombieBehaviour = GetComponent<ZombieBehaviour>();
        InitRigidBodies();
        hashCodeColliders = GetHashCodeOfColliders();
        StartTask(ZombieTaskLog.targetSearch);
        
    }


    protected virtual void FixedUpdate()
    {
    }

    public void SetCivillianController(CivilianController controller)
    {
        civillianController = controller;
    }

    public void AttackTriggerAnimation()
    {
        spawnZombies.InitCivillianDead();
        currentBehaviourState = ZombieBehaviourState.zombieEating;
        SwitchStateBehaviour(currentBehaviourState);
    }

    private void SwitchStateBehaviour(ZombieBehaviourState state)
    {
        if (currentBehaviourState == state) { return; }

        else if (currentBehaviourState != ZombieBehaviourState.none)
        {
            DisableBehaviour(state);
        }

        switch (state)
        {
            case ZombieBehaviourState.zombieRun:
                ZombieRun zombieRun = GetZombieBehaviour(ZombieBehaviourState.zombieRun) as ZombieRun;
                zombieRun.civillianController = civillianController;
                zombieRun.thisZombieBehaviour = thisZombieBehaviour;
                break;
            case ZombieBehaviourState.zombieAttack:
                Debug.Log((int)state + "ZombieAttack!!!");
                break;
            case ZombieBehaviourState.zombieIdle:
                break;
            case ZombieBehaviourState.zombieEating:
                break;
            case ZombieBehaviourState.zombieDie:
                
                break;
        }

        currentBehaviourState = state;
        EnableBehaviour(state);
    }

    private void StartTask(ZombieTaskLog taskLog)
    {
        if (TaskIsAlwaysCompleted(taskLog)) { Debug.LogError("TaskIsAlwaysCompleted"); }

        switch (taskLog)
        {
            case ZombieTaskLog.targetSearch:
                SwitchStateBehaviour(ZombieBehaviourState.zombieRun);
                break;
            case ZombieTaskLog.killCivillian:
                SwitchStateBehaviour(ZombieBehaviourState.zombieAttack);
                break;
        }
        currentTask = taskLog;
    }

    private void GoNextTask(ZombieTaskLog completedTask)
    {
        ZombieTaskLog newTask = ZombieTaskLog.none;


        switch (completedTask)
        {
            case ZombieTaskLog.targetSearch:
                StartTask(ZombieTaskLog.killCivillian);
                newTask = ZombieTaskLog.killCivillian;
                break;
            case ZombieTaskLog.killCivillian:
                break;
        }

        currentTask = newTask;
    }


    private void MarkTaskAsCompleted(ZombieTaskLog taskLog)
    {
        zombieTasksIsCompleted[(int)taskLog] = true;
    }

    public void CompletedTask(ZombieTaskLog taskLog)
    {
        thisZombieBehaviour.DisableBehaviour(currentBehaviourState);
        MarkTaskAsCompleted(taskLog);
        GoNextTask(taskLog);
    }

    private void StopExecutingTask()
    {
        thisZombieBehaviour.DisableBehaviour(currentBehaviourState);
        thisZombieBehaviour.currentTask = ZombieTaskLog.none;
        thisZombieBehaviour.currentBehaviourState = ZombieBehaviourState.zombieDie;
    }

    private bool TaskIsAlwaysCompleted(ZombieTaskLog taskLog)
    {
        return zombieTasksIsCompleted[(int)taskLog];
    }

    private void DisableBehaviour(ZombieBehaviourState state)
    {
        thisZombieBehaviour.zombieBehaviourStates.zombieBehaviourExemplars[(int)state - 1].zombieBehaviour.enabled = false;
    }

    private void EnableBehaviour(ZombieBehaviourState state)
    {

        thisZombieBehaviour.zombieBehaviourStates.zombieBehaviourExemplars[(int)state - 1].zombieBehaviour.enabled = true;
    }

    private ZombieBehaviour GetZombieBehaviour(ZombieBehaviourState state)
    {
        return thisZombieBehaviour.zombieBehaviourStates.zombieBehaviourExemplars[(int)state - 1].zombieBehaviour;
    }

    private HashSet<int> GetHashCodeOfColliders()
    {
        List<Collider> colliders = new List<Collider>(blankEnemy.bones[0].GetComponentsInChildren<Collider>());
        colliders.Add(blankEnemy.bones[0].GetComponent<Collider>());
        HashSet<int> instanceCodes = new HashSet<int>();

        for (int i = 0; i < colliders.Count; i++)
        {
            instanceCodes.Add(colliders[i].GetInstanceID());
        }
        return instanceCodes;
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
        currentBehaviourState = ZombieBehaviourState.zombieIdle;
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
        if (currentBehaviourState == ZombieBehaviourState.zombieDie) { return; }
        StopExecutingTask();
        InitRagdoll();
        
    }

    private void InitRagdoll()
    {
        if (isEnabledRagdoll == true) { return; }
        isEnabledRagdoll = true;
        zombieAnimController.DisableAnimator();
        EnableRigidbodies();
        
    }

    private IEnumerator RunDestroyCharacter(ZombieBodyPartID partID)
    {
        blankEnemy.SetPartIsBroken(partID);
        blankEnemy.DestroyConfigurableJointsOnBodyPart(partID);
        blankEnemy.DisableBrokenBodyPart(partID);
        BodyPartAndPutInObj partAndPutInObj = blankEnemy.GetPartAndPutInObj(partID);
        Transform[] instantiatedBones = blankEnemy.GetInstantiatedBones();
        DestroyAttachedConfigurableJoints(instantiatedBones[0]);

        yield return new WaitForEndOfFrame();

        ZombieBodyControl[] zombieBodyControls = blankEnemy.GetZombieBodyControls(instantiatedBones);
        //Debug.Log(blankEnemy.PartIsAttachedToRope(partAndPutInObj));
        if (blankEnemy.PartIsAttachedToRope(partAndPutInObj) == true)
        {
            int[] indexesInHirachly = blankEnemy.GetBonesIndexesFromHirachly(partAndPutInObj);
            Transform[] comparableBones = blankEnemy.GetBonesFromHirachlyRootBone(instantiatedBones[0], indexesInHirachly);
            List<List<ConnectedRope>> connectedRopes = blankEnemy.GetConnectedsRopesFromBodyPart(partAndPutInObj);
            ReconnectBodyPartsToRopes(comparableBones, connectedRopes);
        }

        blankEnemy.PrepareBonesForSlicedPart(zombieBodyControls, partID);
        blankEnemy.ReBuildHirachlyBones(instantiatedBones[0], zombieBodyControls, partID);
        DisableZombieBodyPartsControllers(partAndPutInObj);

        RelinkSlicedParts(instantiatedBones, partAndPutInObj);

        yield return null;
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



       /* RopeBehaviour ropeBehaviour = zombieBodyControl.currentRopeBehaviour;
        Rigidbody rigidbody = comparableBone.GetComponent<Rigidbody>();
        ropeBehaviour.SetNewRigidbodyToSelectedJoint(zombieBodyControl.ownOrderInRope, rigidbody);*/
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

    protected void RotateToPlayer()
    {
        Vector3 playerCamPos = civillianController.transform.position;
        Vector3 normaDirToPlayer = (playerCamPos - transform.position).normalized;
        float yTargetRotate = Mathf.Atan2(normaDirToPlayer.x, transform.forward.z) * Mathf.Rad2Deg;
        yRotate = Mathf.MoveTowards(yRotate, yTargetRotate, 20f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, yRotate, 0);
    }

    protected void MoveToPlayer()
    {
        if (isInterCollisionWithOther == true) { return; }
        Vector3 playerPos = civillianController.transform.position;
        playerPos.y = 0f;
        Vector3 pos = Vector3.MoveTowards(transform.position, playerPos, Time.deltaTime/2f);
        transform.position = pos;
    }

    protected float CalculateDistanceToCivillian()
    {
        return Vector3.Distance(transform.position, civillianController.transform.position);
    }
}
