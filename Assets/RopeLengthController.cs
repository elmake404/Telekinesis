using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RopeLengthController : MonoBehaviour
{
    public UIMeshedSpiral uIMeshedSpiral;
    public GameObject circle;
    private float canvasScale;
    private float yOffsetSpiral = 100f;

    private void Start()
    {
        canvasScale = GeneralManager.instance.canvas.scaleFactor;
        yOffsetSpiral *= canvasScale;
        
    }

    private void FixedUpdate()
    {
        Vector3 followPos = Input.mousePosition;
        followPos.y += yOffsetSpiral;
        transform.position = followPos;
    }

    public void EnableSensor()
    {
        transform.position = Input.mousePosition;
        this.enabled = true;
        uIMeshedSpiral.gameObject.SetActive(true);
        circle.SetActive(true);
        uIMeshedSpiral.sliderToggle = 100;
    }

    public void DisableSensor()
    {
        this.enabled = false;
        uIMeshedSpiral.gameObject.SetActive(false);
        circle.SetActive(false);
    }

    public void SetFillSpiral(int num)
    {
        uIMeshedSpiral.sliderToggle = num;
        uIMeshedSpiral.UpdateMesh();

    }
}
