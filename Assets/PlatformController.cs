using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform interactObjectPlaceholder;
    public Transform playerLocate;
    public SpawnZombies spawnZombies;

    void Start()
    {
        EnableInteractObject();
        EnableSpawnZombie();
    }

    private void EnableInteractObject()
    {
        IInitObject[] initObjects = interactObjectPlaceholder.GetComponentsInChildren<IInitObject>();
        for (int i = 0; i < initObjects.Length; i++)
        {
            initObjects[i].InitComponent();
        }
    }

    private void EnableSpawnZombie()
    {
        spawnZombies.enabled = true;
    }

    public void DisableThisPlatform()
    {
        gameObject.SetActive(false);
    }
}
