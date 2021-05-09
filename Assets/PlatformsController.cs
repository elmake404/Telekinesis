using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformsController : MonoBehaviour
{
    public PlatformController[] platformControllers;
    private int currentPlatformIndex;

    private void Start()
    {
        EnableFirsPlatform();
    }

    private void EnableFirsPlatform()
    {
        platformControllers[0].enabled = true;
        currentPlatformIndex = 0;
    }

    public void TimeToChangePlatform()
    {
        if (currentPlatformIndex + 1 > platformControllers.Length)
        {
            return;
        }

        GeneralManager.instance.playerController.SetCurrentAndDestinationLoc(platformControllers[currentPlatformIndex].playerLocate, platformControllers[currentPlatformIndex + 1].playerLocate);
        GeneralManager.instance.playerController.SwitchStateAction(PlayerState.playerIsMove);
        //currentPlatformIndex += 1;
    }

    public Transform GetFirstLocatePlayer()
    {
        return platformControllers[0].playerLocate;
    }

    public int GetCurrentIndexPlatform()
    {
        return currentPlatformIndex;
    }

    public void AddIndexToCurrentPlatform()
    {
        currentPlatformIndex += 1;
    }
}
