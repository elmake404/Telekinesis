using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CivilianRun : MonoBehaviour
{
    public CivilianController civilianController;
    private float lerpTime = 3f;
    private float currentLerpTime;
    private float yRotate = 0f;
    private Vector3 startPos;
    

    [HideInInspector] public Vector3 targetPos;

    void Start()
    {
        startPos = transform.position;
    }

    
    void Update()
    {
        RotateCivillian();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentLerpTime = 0f;
        }

        //increment timer once per frame
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            //currentLerpTime = lerpTime;
            civilianController.ChangeCivillianState(CivillianState.civillPanic);
            this.enabled = false;
        }

        //lerp!
        float perc = currentLerpTime / lerpTime;
        transform.position = Vector3.Lerp(startPos, targetPos, perc);
    }

    private void RotateCivillian()
    {
        Vector3 playerCamPos = targetPos;
        Vector3 normaDirToPlayer = (playerCamPos - transform.position).normalized;
        float yTargetRotate = Mathf.Atan2(normaDirToPlayer.x, transform.forward.z) * Mathf.Rad2Deg;
        yRotate = Mathf.MoveTowards(yRotate, yTargetRotate, 50f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, yRotate, 0);
    }

}

