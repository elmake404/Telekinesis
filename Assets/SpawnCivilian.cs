using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CivillianState
{
    civillRun,
    civillPanic,
    civilDead,
    civilSaved
}

public enum CivillianAnimState
{
    panicRun,
    panicIdle,
    idle,
    death
}

public class SpawnCivilian : MonoBehaviour
{
    public Transform restPoint;
    public GameObject civilianBones;
    public SpawnZombies spawnZombies;
    public GameObject helpParticles;
    [HideInInspector] public CivilianController createdCivilian;

    private void Start()
    {
        CreateCivilian();
        
    }

    private void CreateCivilian()
    {
        GameObject civilian = Instantiate(civilianBones);
        CivilianController civilianController = civilian.GetComponent<CivilianController>();
        civilianController.restPoint = restPoint;
        civilianController.platformPos = GetPlatformWorldPos();
        civilianController.enabled = true;
        spawnZombies.SetCivillianController(civilianController);

        civilianController.spawnCivilian = this;

        CivilianBlank civilianBlank = civilian.GetComponent<CivilianBlank>();
        CivilianConstructor civilianConstructor = GeneralManager.instance.civilianConstructor;
        GameObject head = Instantiate(civilianConstructor.GetRandomHead(), civilianBlank.headPlaceholder);
        head.SetActive(true);
        head.transform.localRotation = Quaternion.Euler(Vector3.zero);
        GameObject body = Instantiate(civilianConstructor.GetRandomBody(), civilianBlank.bodyPlaceholder);
        body.SetActive(true);
        SkinnedMeshRenderer skinnedMeshRenderer = body.GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.rootBone = civilianBlank.rootBones;
        skinnedMeshRenderer.bones = civilianBlank.bones;

        createdCivilian = civilianController;
    }

    private Vector3 GetPlatformWorldPos()
    {
        return transform.parent.position;
        
    }

    public void SetCivillianDead()
    {
        createdCivilian.ChangeCivillianState(CivillianState.civilDead);
    }

    public void SetCivilianIsSaved()
    {
        createdCivilian.ChangeCivillianState(CivillianState.civilSaved);
    }

    public void RunPlayHelpParticles(Vector3 spawnPos)
    {
        StartCoroutine(PlayHelpParticles(spawnPos));
    }

    private IEnumerator PlayHelpParticles(Vector3 spawnPos)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject instance = Instantiate(helpParticles);
        instance.transform.position = spawnPos;
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        float duration = particleSystem.main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(instance);
        
    }
}
