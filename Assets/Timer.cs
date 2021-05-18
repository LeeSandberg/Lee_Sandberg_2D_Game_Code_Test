// Written by Lee Sandberg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    public Text timerText;

    private TimeSpan    timePlaying;
    private bool        timerGoing;
    private float       elapsedTime;
    private string      timePlayStr;
    private float       startTime;

    public float GetElapsedTimeAsFloat()
    {
        return elapsedTime;
    }
    
    public string GetElapsedTimeAsString()
    {
        return timePlayStr; ;
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update.
    void Start()
    {
        timerText.text = "Go !";
        timerGoing = false;
    }

    public void Init()
    {
        Start();
    }

    public void BeginTimer()
    {
        timerGoing = true;
        startTime = Time.time;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
    }

    public String ElapsedTimeFloatToString(float elapsedTime)
    {
        timePlaying = TimeSpan.FromSeconds(elapsedTime);
        timePlayStr = timePlaying.ToString("mm':'ss");
        return timePlayStr;
    }

    private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlayStr = ElapsedTimeFloatToString(elapsedTime);
            timerText.text = timePlayStr;
            yield return null; // Get back to rendering the rest of the frame.
        }
    }
}
