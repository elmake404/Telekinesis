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
        if (currentPlatformIndex + 1 >= platformControllers.Length)
        {
            GeneralManager.instance.canvasManager.DelayActivateWinBlock();
            return;
        }

        GeneralManager.instance.playerController.SwitchStateAction(PlayerState.playerIsMove);
    }


    public int GetCurrentIndexPlatform()
    {
        return currentPlatformIndex;
    }

    public void SwitchToTheNextPlatform()
    {
        currentPlatformIndex += 1;
        ActivatePlatform(currentPlatformIndex);
    }

    private void ActivatePlatform(int index)
    {
        platformControllers[index].enabled = true;
    }
}
