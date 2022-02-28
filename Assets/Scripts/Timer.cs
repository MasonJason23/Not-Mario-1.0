using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private float accumulatedTime;
    private TimeSpan totalTime;

    private void Start()
    {
        totalTime = new TimeSpan(0, 0, 100);
    }

    void Update()
    {
        if (MainCharacterController.gameEndFlag)
        {
            return;
        }
        accumulatedTime += Time.deltaTime;

        if (accumulatedTime >= 1f)
        {
            accumulatedTime = 0f;
            totalTime = totalTime.Subtract(new TimeSpan(0, 0, 1));

            var timeTxt = GetComponent<TextMeshProUGUI>();
            timeTxt.text = totalTime.TotalSeconds.ToString();
        }

        if (totalTime.TotalSeconds <= 0)
        {
            Debug.Log("Time is up! You Lose!");
            MainCharacterController.gameEndFlag = true;
        }
    }
}
