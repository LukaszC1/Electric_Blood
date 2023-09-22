using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTime : MonoBehaviour
{
    public float time;
    TimerUi timer;


    private void Awake()
    {
        timer = FindObjectOfType<TimerUi>();
    }
    private void Update()
    {
        time += Time.deltaTime;
        timer.UpdateTime(time);
    }
}
