using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TimerClass
{
    public bool isTimerRunning = false;
    private float timeElapsed = 0.0f;
    private float currentTime = 0.0f;
    private float lastTime = 0.0f;
    private float timeScaleFactor = 1.0f;

    public void UpdateTimer()
    {
        timeElapsed = Mathf.Abs(Time.realtimeSinceStartup - lastTime);

        if (isTimerRunning)
        {
            currentTime += timeElapsed * timeScaleFactor;
        }

        lastTime = Time.realtimeSinceStartup;

    }

    public void StartTimer()
    {
        isTimerRunning = true;
        lastTime = Time.realtimeSinceStartup;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        lastTime = Time.realtimeSinceStartup;
        currentTime = 0f;
    }

    public float GetTime()
    {
        UpdateTimer();
        return currentTime;
    }

    public void SetTimer(float timeToSet)
    {

        currentTime = timeToSet;

    }


}
