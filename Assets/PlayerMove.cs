using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public PlayerController playerController;
    private Vector3 startPos;
    private Vector3 endPos;
    float currentLerpTime = 0f;
    float lerpTime = 2f;
    private bool isSwitchPlatform = false;

    private void OnEnable()
    {
        currentLerpTime = 0f;
        isSwitchPlatform = false;
    }

    private void Start()
    {
        currentLerpTime = 0f;
    }

    void Update()
    {
        currentLerpTime += Time.deltaTime;

        if (currentLerpTime > 1.0f && isSwitchPlatform == false)
        {
            Debug.Log("Switch");
            isSwitchPlatform = true;
            GeneralManager.instance.platformsController.SwitchToTheNextPlatform();
        }

        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
            playerController.SwitchStateAction(PlayerState.playerIsNormal);
            GeneralManager.instance.platformsController.platformControllers[GeneralManager.instance.platformsController.GetCurrentIndexPlatform()].DisableThisPlatform();
            //GeneralManager.instance.platformsController.SwitchToTheNextPlatform();
            GeneralManager.instance.platformsController.AddIndexToCurrentPlatform();
            DisablePlayerMove();
        }

        

        float t = currentLerpTime / lerpTime;
        t = t * t * t * (t * (6f * t - 15f) + 10f);
        transform.position = Vector3.Lerp(startPos, endPos, t);


    }


    public void SetCurrentLocateAndDestination(Vector3 current, Vector3 destination)
    {
        this.endPos = destination;
        this.startPos = current;
    }

    public void EnablePlayerMove()
    {
        this.enabled = true;
    }
    public void DisablePlayerMove()
    {
        this.enabled = false;
    }

   
}
