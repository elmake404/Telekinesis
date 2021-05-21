using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionControl : MonoBehaviour
{
    private float slowFactor = 0.05f;
    private float cacheFixedDeltaTime = 0f;
    public bool isSlowMotion = false;

    private void Start()
    {
        cacheFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void StartRunSlowMotion()
    {
        StartCoroutine(RunSlowMotion());
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void ContinueTime()
    {
        Time.timeScale = 1f;
    }

    private IEnumerator RunSlowMotion()
    {
        isSlowMotion = true;
        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = cacheFixedDeltaTime;
        isSlowMotion = false;
        yield return null;
    }
    
}
