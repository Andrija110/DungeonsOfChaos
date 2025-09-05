using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float time;
    private float timeStep;
    private bool timerIsRunning=false;
    private PlayerStats playerStats = PlayerStats.Instance;

    public int Value { get; set; }
    public delegate void OnValueChangedDelegate(int value);
    public OnValueChangedDelegate onValueChangedCallback;

    public void StartCountDown(int initValue, float seconds)
    {
        Value = initValue;
        timerIsRunning = true;
        time = 0;
        timeStep = seconds;
    }

    public void StopCountDown()
    {
        timerIsRunning = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            time += Time.deltaTime;
            if (time > timeStep) {
                Value--;
                time = time - timeStep;
                onValueChangedCallback?.Invoke(Value);
            }
        }
    }
}
