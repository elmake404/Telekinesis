using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform interactObjectPlaceholder;
    public Transform playerLocate;
    public SpawnZombies spawnZombies;
    public SpawnCivilian spawnCivilian;

    void Start()
    {
        StartCoroutine(SequentialEnableElements());
    }

    private IEnumerator SequentialEnableElements()
    {
        EnableSpawnCivillian();
        yield return new WaitForEndOfFrame();
        EnableSpawnZombie();
        yield return new WaitForEndOfFrame();
        EnableInteractObject();
        yield return null;
    }

    private void EnableInteractObject()
    {
        IInitObject[] initObjects = interactObjectPlaceholder.GetComponentsInChildren<IInitObject>();
        for (int i = 0; i < initObjects.Length; i++)
        {
            
            initObjects[i].InitComponent();
        }

        AddInteractObjToRendererCobtainer();
    }

    private void AddInteractObjToRendererCobtainer()
    {
        Renderer[] renderers = interactObjectPlaceholder.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            TemporaryRendererContainer.instance.AddRendererToPost(renderers[i]);
        }
    }

    private void EnableSpawnZombie()
    {
        spawnZombies.enabled = true;
    }

    private void EnableSpawnCivillian()
    {
        spawnCivilian.enabled = true;
    }

    public void DisableThisPlatform()
    {
        gameObject.SetActive(false);
    }


}
