using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLengthController : MonoBehaviour
{
    public UIMeshedSpiral uIMeshedSpiral;

    public void EnableSensor()
    {
        gameObject.SetActive(true);
        uIMeshedSpiral.sliderToggle = 100;
    }

    public void DisableSensor()
    {
        gameObject.SetActive(false);
    }
}
