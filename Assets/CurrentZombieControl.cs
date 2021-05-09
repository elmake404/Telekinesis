using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieControllerState
{
    zombieEmerge,
    zombieRun,
    zombieAttack,
    zombieDie
}

public class CurrentZombieControl : MonoBehaviour
{
    public BlankEnemy blankEnemy;
    public ZombieAnimController zombieAnimController;
    private bool isEnabledRagdoll = false;
    public bool isInterCollisionWithOther = false;

    private PlayerController playerController;
    private float yRotate = 0;
    private float minDistanceToPlayer = 2f;
    private Collider[] allChildrenColliders;
    private SpawnZombies spawnZombies;

    private ZombieControllerState controllerState = ZombieControllerState.zombieEmerge;

    private void Start()
    {
        InitPlayerController();
        StartCoroutine(WaitZombieEmergeAnim());
    }

    private void FixedUpdate()
    {
        SwitchStateEnemy(controllerState);
    }

    private void SwitchStateEnemy(ZombieControllerState state)
    {
        switch (state)
        {
            case ZombieControllerState.zombieEmerge:
                break;
            case ZombieControllerState.zombieRun:
                MoveToPlayer();
                RotateZombie();
                WaitToAttackState();
                break;
            case ZombieControllerState.zombieAttack:
                RotateZombie();
                zombieAnimController.SetAnimation(ZombieAnimationState.isAttack);
                break;
            case ZombieControllerState.zombieDie:
                break;
        }
    }

    public void AddExplosionForceToBody(Vector3 source)
    {
        for (int i = 0; i < allChildrenColliders.Length; i++)
        {
            if (allChildrenColliders[i].enabled == false) { continue; }
            allChildrenColliders[i].attachedRigidbody.AddExplosionForce(3f, source, 1f, 5f, ForceMode.Impulse);
        }
    }

    public Transform GetHeadTransform()
    {
        return blankEnemy.headContainer;
    }

    public void EnableRagdoll()
    {
        if (isEnabledRagdoll == true) { return; }
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
        SetDefaultLayersToAllColliders();
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

    private void InitPlayerController()
    {
        playerController = GeneralManager.instance.playerController;
    }

    private void RotateZombie()
    {
        Vector3 playerCamPos = playerController.transform.position;
        Vector3 normaDirToPlayer = (playerCamPos - transform.position).normalized;
        float yTargetRotate = Mathf.Atan2(normaDirToPlayer.x, transform.forward.z) * Mathf.Rad2Deg;
        yRotate = Mathf.MoveTowards(yRotate, yTargetRotate, 8f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, yRotate, 0);
    }
    private void MoveToPlayer()
    {
        if (isInterCollisionWithOther == true) { return; }
        Vector3 playerPos = playerController.transform.position;
        playerPos.y = 0f;
        Vector3 pos = Vector3.MoveTowards(transform.position, playerPos, Time.deltaTime/3f);
        transform.position = pos;
    }

    private float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerController.transform.position);
    }

    private void WaitToAttackState()
    {
        if (CalculateDistanceToPlayer() < minDistanceToPlayer)
        {
            controllerState = ZombieControllerState.zombieAttack;
        }
    }

    private IEnumerator WaitZombieEmergeAnim()
    {
        float duration = 1.5f;
        yield return new WaitForSeconds(duration);
        EnableAndSetAllColliders();
        controllerState = ZombieControllerState.zombieRun;
        zombieAnimController.SetAnimation(ZombieAnimationState.isWalk);
        yield return null;

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

    private void SetDefaultLayersToAllColliders()
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
