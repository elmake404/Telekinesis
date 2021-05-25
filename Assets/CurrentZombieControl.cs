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
    public BlankEnemy blankEnemy;
    public ZombieAnimController zombieAnimController;
    [HideInInspector] public GameObject headLink;
    private bool isEnabledRagdoll = false;
    [HideInInspector] public bool isInterCollisionWithOther = false;
    public ConnectedPin connectedPin;
    [HideInInspector] public CivilianController civillianController;
    private float yRotate = 0;
    private float minDistanceToPlayer = 1f;
    private Collider[] allChildrenColliders;
    private SpawnZombies spawnZombies;
    public bool isRopeBreak = false;
    public bool isPinned = false;
    public GameObject particlesOnHit;
    public GameObject directParticlesOnHit;
    private ZombieControllerState controllerState = ZombieControllerState.zombieRun;

    private void Start()
    {
        EnableAndSetAllColliders();
    }

    private void FixedUpdate()
    {
        SwitchStateEnemy(controllerState);
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

    public void MakeIdleZombie()
    {
        controllerState = ZombieControllerState.zombieIdle;
    }

    public void AddExplosionForceToBody(Vector3 source)
    {
        for (int i = 0; i < allChildrenColliders.Length; i++)
        {
            if (allChildrenColliders[i].enabled == false) { continue; }
            allChildrenColliders[i].attachedRigidbody.AddExplosionForce(3f, source, 5f, 10f, ForceMode.Impulse);
        }
    }

    public void IgnoreRopeColliders(Collider[] colliders)
    {

        for (int i = 0; i < allChildrenColliders.Length; i++)
        {
            for (int k = 0; k < colliders.Length; k++)
            {
                Physics.IgnoreCollision(allChildrenColliders[i], colliders[k]);
            }
        }
    }

    public bool GetIsEnabledRagdoll()
    {
        return isEnabledRagdoll;
    }

    public Transform GetHeadTransform()
    {
        return blankEnemy.headContainer;
    }

    public void EnableRagdoll()
    {
        if (isEnabledRagdoll == true) { return; }
        GeneralManager.instance.slowMotionControl.StartRunSlowMotion();
        controllerState = ZombieControllerState.zombieDie;
        zombieAnimController.DisableAnimator();
        spawnZombies.AddNumOfDeadZombies();

        isEnabledRagdoll = true;
        Rigidbody[] rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
        
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = false;
            rigidbodies[i].useGravity = true;
        }
        
    }

    private IEnumerator DelayedDeath()
    {
        
        yield return new WaitForSeconds(1.5f);
        EnableRagdoll();
        
    }

    public void StartRoutineDelayDeath()
    {
        StartCoroutine(DelayedDeath());
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


    private void EnableAndSetAllColliders()
    {
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        allChildrenColliders = colliders;

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }

    }

    public void SetDefaultLayersToAllColliders()
    {
        for (int i = 0; i < allChildrenColliders.Length; i++)
        {
            allChildrenColliders[i].transform.gameObject.layer = 0;
        }
    }

    public void SetSpawnZombies(SpawnZombies spawnZombies)
    {
        this.spawnZombies = spawnZombies;
    }
    
}
