using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureScript : MonoBehaviour {

    AirplaneController airplane;

    public float maxTemp;               // Temperature indicates durability degeneration rate. The higher the Temp, the faster the durability degenerates.
    public float currentTemp;
    public float tempAmp;               // Controls the rate that temperature increases or decreases.

    [HideInInspector]
    public float gaugeRotation;


    void Start () {
        airplane = GetComponent<AirplaneController>();
	}
	
	void FixedUpdate () {
		if (airplane.enableTemp)
        {
            ManageTemp();
            GaugeCheck();
        }
	}

    // Calculates the current temperature.
    void ManageTemp()
    {
        if (currentTemp < maxTemp)
            IncreaseTemp();
        else if (currentTemp >= maxTemp)
            currentTemp = maxTemp;
        else
            currentTemp = 0f;
    }

    // Increases temperature over time.
    void IncreaseTemp()
    {
        currentTemp += tempAmp * Time.deltaTime;
    }

    // Decreases temperature over time. (Currently not being used.)
    void DecreaseTemp()
    {
        currentTemp -= tempAmp * Time.deltaTime;
    }

    // Provides the engine gauge with a rotation value between 0 and 1.
    void GaugeCheck()
    {
        gaugeRotation = currentTemp / maxTemp;
    }
}
