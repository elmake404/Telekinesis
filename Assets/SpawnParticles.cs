using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnParticles
{
    public static void InSpecifiedPoint(MonoBehaviour refForStartCoroutine ,GameObject particles, Vector3 pos)
    {
        GameObject instance = GameObject.Instantiate(particles);
        instance.transform.position = pos;
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        float duration = particleSystem.main.duration;
        refForStartCoroutine.StartCoroutine(DeleteParticlesAfterLifeTime(instance, duration));
    }

    public static void InParent(MonoBehaviour refForStartCoroutine, GameObject particles, Transform parent)
    {
        GameObject instance = GameObject.Instantiate(particles);
        instance.transform.position = parent.position;
        instance.transform.SetParent(parent);
        ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
        float duration = particleSystem.main.duration;
        refForStartCoroutine.StartCoroutine(DeleteParticlesAfterLifeTime(instance, duration));
    }

    private static IEnumerator DeleteParticlesAfterLifeTime(GameObject particles, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        GameObject.Destroy(particles);
        yield return null;
    }
}
