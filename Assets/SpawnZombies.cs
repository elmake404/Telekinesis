using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZombies : MonoBehaviour
{
    public GameObject zombiePrefabBones;
    public Transform[] posSpawn;
    public GameObject spawnZombieParticles;
    private int numOfPosChanges = 0;


    private void Start()
    {
        StartCoroutine(DelaySpawnZombies());
    }

    private void CreateEnemy()
    {

        GameObject instanceZombie = Instantiate(zombiePrefabBones);
        BlankEnemy blankEnemy = instanceZombie.GetComponent<BlankEnemy>();

        GameObject head = GeneralManager.instance.zombieConstructor.GetLinkToRandomHead();
        GameObject body = GeneralManager.instance.zombieConstructor.GetLinkToRandomBody();

        GameObject instanceBody = Instantiate(body, blankEnemy.bodyContainer);
        SkinnedMeshRenderer skinnedMeshRenderer = instanceBody.GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.rootBone = blankEnemy.rootBones;
        skinnedMeshRenderer.bones = blankEnemy.GetBones();

        Instantiate(head, blankEnemy.headContainer);

        Vector3 posSpawn = GetPosSpawn();
        instanceZombie.transform.position = posSpawn;
        Vector3 particlesPos = posSpawn;
        particlesPos.y = + 0.050f;
        StartCoroutine(PlaySpawnZombieParticles(particlesPos));
        
    }

    private Vector3 GetPosSpawn()
    {
        Vector3 pos = posSpawn[numOfPosChanges].position;
        numOfPosChanges += 1;
        return pos;
    }

    private IEnumerator DelaySpawnZombies()
    {
        for (int i = 0; i < posSpawn.Length; i++)
        {
            CreateEnemy();
            yield return new WaitForSeconds(0.50f);
        }

        yield return null;
    }

    private IEnumerator PlaySpawnZombieParticles(Vector3 pos)
    {
        GameObject particles = Instantiate(spawnZombieParticles);
        particles.transform.position = pos;
        ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
        float duration = particleSystem.main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(particles);
        yield return null;
    }
}
