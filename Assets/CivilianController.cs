using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CivilianController : MonoBehaviour
{
    public CivilianRun civilianRun;
    public CivillianAnimController civillianAnimController;
    public GameObject heartsParticles;
    [HideInInspector] public Transform restPoint;
    [HideInInspector] public Vector3 platformPos;
    private CivillianState civillianState = CivillianState.civillRun;

    
    void Start()
    {
        RunRunCivilianToRestPoint();

    }

    private void RunRunCivilianToRestPoint()
    {
        Vector3 restPos = restPoint.position;
        Vector3 startPos = platformPos;

        transform.position = startPos;
        civilianRun.targetPos = restPos;
        civilianRun.enabled = true;

    }

    public void ChangeCivillianState(CivillianState state)
    {
        if(state == civillianState) { return; }

        switch (state)
        {
            case CivillianState.civillRun:
                break;
            case CivillianState.civillPanic:
                civillianAnimController.ChangeAnimState(CivillianAnimState.panicIdle);
                break;
            case CivillianState.civilDead:
                civillianAnimController.ChangeAnimState(CivillianAnimState.death);
                break;
            case CivillianState.civilSaved:
                StartCoroutine(PlayHeartsParticles());
                civillianAnimController.ChangeAnimState(CivillianAnimState.idle);
                break;
        }
    }

    private IEnumerator PlayHeartsParticles()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject particles = Instantiate(heartsParticles);
        Vector3 spawnPos = transform.position;
        spawnPos.y += 3f;
        particles.transform.position = spawnPos;
        float duration = particles.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(particles);
    }
}
