using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public PlayerController playerController;
    private Transform destinationPoint;
    private Transform currentPlayerLocate;
    private Vector3 startPos;
    private Vector3 endPos;
    float currentLerpTime = 0f;
    float lerpTime = 4f;

    private void Start()
    {
        currentLerpTime = 0f;
        SetEndAndStartPos();
    }

    void Update()
    {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
            playerController.SwitchStateAction(PlayerState.playerIsNormal);
            GeneralManager.instance.platformsController.platformControllers[GeneralManager.instance.platformsController.GetCurrentIndexPlatform()].DisableThisPlatform();
            GeneralManager.instance.platformsController.AddIndexToCurrentPlatform();
            DisablePlayerMove();
        }

        float t = currentLerpTime / lerpTime;
        t = t * t * t * (t * (6f * t - 15f) + 10f);
        transform.position = Vector3.Lerp(startPos, endPos, t);


    }

    private void SetEndAndStartPos()
    {
        startPos = new Vector3(currentPlayerLocate.position.x, transform.position.y, currentPlayerLocate.position.z);
        endPos = new Vector3(destinationPoint.position.x, transform.position.y, destinationPoint.position.z);
        
    }

    public void SetCurrentLocateAndDestination(Transform current, Transform destination)
    {
        this.destinationPoint = destination;
        this.currentPlayerLocate = current;
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
