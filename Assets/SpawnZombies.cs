﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZombies : MonoBehaviour
{
    public GameObject zombiePrefabBones;
    public Transform[] posSpawn;
    public GameObject spawnZombieParticles;
    public PlatformController platformController;
    public SpawnCivilian spawnCivilian;
    private List<ZombieBehaviour> linksToSpawnedZombies = new List<ZombieBehaviour>();
    private CivilianController civilianController;
    private int numOfPosChanges = 0;
    private int numOfDeadZombies = 0;

    private void Start()
    {
        SpawnCreatEnemy();
    }

    private void CreateEnemy()
    {
        GameObject instanceZombie = Instantiate(zombiePrefabBones);
        ZombieBehaviour currentZombieControl = instanceZombie.GetComponent<ZombieBehaviour>();
        linksToSpawnedZombies.Add(currentZombieControl);
        currentZombieControl.spawnZombies = this;
        currentZombieControl.SetCivillianController(civilianController);
        Vector3 posSpawn = GetPosSpawn();
        instanceZombie.transform.position = posSpawn;
    }

    private Vector3 GetPosSpawn()
    {
        Vector3 pos = posSpawn[numOfPosChanges].position;
        numOfPosChanges += 1;
        return pos;
    }

    private void SpawnCreatEnemy()
    {
        for (int i = 0; i < posSpawn.Length; i++)
        {
            CreateEnemy();
        }  
    }

    public void StopAnotherZombies(int currentHash)
    {
        for (int i = 0; i < linksToSpawnedZombies.Count; i++)
        {
            if (currentHash == linksToSpawnedZombies[i].GetHashCode()) { continue; }

            linksToSpawnedZombies[i].MakeIdleZombie();
        }
    }

    public void AddNumOfDeadZombies()
    {
        numOfDeadZombies += 1;

        if (numOfDeadZombies >= posSpawn.Length)
        {
            TemporaryRendererContainer.instance.ClearTemporaryRenderers();
            InitSavedCivillians();
            StartCoroutine(DelayStartMoveToNextPlatform());
            StartCoroutine(LazyDeleteAllSpawnedZombies());

        }
    }

    public void InitCivillianDead()
    {
        spawnCivilian.SetCivillianDead();
    }

    public void SetCivillianController(CivilianController civilianController)
    {
        this.civilianController = civilianController;
    }

    public void InitSavedCivillians()
    {
        spawnCivilian.SetCivilianIsSaved();
    }

    private IEnumerator DelayStartMoveToNextPlatform()
    {
        yield return new WaitForSeconds(2f);
        GeneralManager.instance.platformsController.TimeToChangePlatform();
        yield return null;
    }

    private IEnumerator LazyDeleteAllSpawnedZombies()
    {
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < linksToSpawnedZombies.Count; i++)
        {
            Destroy(linksToSpawnedZombies[i].gameObject);
            yield return new WaitForSeconds(0.4f);    
        }
        yield return null;
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



[System.Serializable] public struct BodyPartAndPutInObj
{
    public ZombieBodyPartID zombieBodyPartID;
    public ZombieBodyControl[] zombieBodyControls;
    public GameObject[] usedBodyParts;
}

[System.Serializable] public struct ZombieBehaviourStates
{
    public ZombieBehaviourExemplar[] zombieBehaviourExemplars;
}

[System.Serializable] public struct ZombieBehaviourExemplar
{
    public ZombieBehaviourState behaviourState;
    public ZombieBehaviour zombieBehaviour;
}
