using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeScript : MonoBehaviour {

    // Setup
    PlayerController player;
    AirplaneController airplane;
    TurnCrankScript turnCrank;

    public GameObject inputObject;              // The object/controller that'll help manipulate the gauge.

    // Gauge Functionality
    public bool isEngine;                       // Flag if the gauge is hooked up to engine.
    public bool isLeftWing;                     // Flag if the gauge is hooked up to the left wing.
    public bool isRightWing;                    // Flag if the gauge is hooked up to the right wing.
    public bool isHealth;                       // Flag if the gauge is hooked up to health points.
    public bool isTemp;                         // Flag if the gauge is hooked up to temperature.

    float minValue;                             // Minimum value of vehicle part.
    float maxValue;                             // Maximum value of vehicle part.
    float curValue;                             // Current value of vehicle part.

    public float minGaugeDegree;                // Determines the gauge pointer's rotation at minimum value.
    public float maxGaugeDegree;                // Determines the gauge pointer's rotation at maximum value.
    public float curGaugeDegree;                     // Do not edit. The gauge pointer's current rotation.

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
    {
        player = FindObjectOfType<PlayerController>();
        airplane = FindObjectOfType<AirplaneController>();

        if (isLeftWing || isRightWing)
            turnCrank = inputObject.GetComponent<TurnCrankScript>();
        else
            turnCrank = null;
        /*
        if (isEngine)
            tapEngine = inputObject.GetComponent<TapEngineScript>();
        */
    }
	
	void FixedUpdate()
    {
        CalculateGaugeRotation(turnCrank.rotationGoalCurrent);
	}

    void FindVehiclePart()
    {
        if (isEngine)
        {
            minValue = 0f;
            maxValue = airplane.engineMaxDur;
            curValue = airplane.engineCurrentDur;
        }

        if (isLeftWing)
        {
            minValue = 0f;
            maxValue = airplane.leftMaxDur;
            curValue = airplane.leftCurrentDur;
        }

        if (isRightWing)
        {
            minValue = 0f;
            maxValue = airplane.rightCurrentDur;
            curValue = airplane.rightCurrentDur;
        }

        if (isHealth)
        {
            minValue = 0f;
            maxValue = airplane.maxHealth;
            curValue = airplane.currentHealth;
        }

        if (isTemp)
        {
            //minValue = 0f;
            //maxValue = airplane.maxTemp;
            //curValue = airplane.currentTemp;
        }
    }

    void CalculateGaugeRotation(float rotGoalCurrent)
    {
        transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, curGaugeDegree));

        if (isLeftWing || isRightWing)
        {
            if (turnCrank.crankInteractable)
            {
                curValue = rotGoalCurrent / turnCrank.rotationGoalAmount;
                curGaugeDegree = curValue * maxGaugeDegree;
            }
            else
            {
                FindVehiclePart();
                curGaugeDegree = curValue / maxValue;
            }

        }
    }
}
