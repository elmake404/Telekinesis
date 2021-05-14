using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionControl : MonoBehaviour
{
    private float slowFactor = 0.05f;
    private float cacheFixedDeltaTime = 0f;

    private void Start()
    {
        cacheFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void StartRunSlowMotion()
    {
        StartCoroutine(RunSlowMotion());
    }

    private IEnumerator RunSlowMotion()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = cacheFixedDeltaTime;
        yield return null;
    }
    
}
