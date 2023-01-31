using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public Text CounterText;

    private int Count = 0;
    private float elapsedTime = 0f;

    private void Start()
    {
        Count = 0;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("elapsings time: " + elapsedTime);
    }

    /*Leaving the counting logic here may still work in the end because there should be only one box at the end of the flickering phase*/
    private void OnTriggerEnter(Collider other)
    {
        Count += 1;
        CounterText.text = "Count : " + Count;
    }

    public int ReturnCount()
    {
        return Count;
    }


}
