using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeScript : MonoBehaviour {

    // Setup
    HealthScript health;
    TemperatureScript temp;
    TurnCrankScript crank;
    SmashEngineScript engine;

    public GameObject inputObject;              // The object/controller that'll help manipulate the gauge.

    // Gauge Functionality
    public bool isEngine;                       // Flag if the gauge is hooked up to engine.
    public bool isLeftWing;                     // Flag if the gauge is hooked up to the left wing.
    public bool isRightWing;                    // Flag if the gauge is hooked up to the right wing.
    public bool isHealth;                       // Flag if the gauge is hooked up to health points.
    public bool isTemp;                         // Flag if the gauge is hooked up to temperature.

    public float minGaugeDegree;                // Determines the gauge pointer's rotation at minimum value.
    public float maxGaugeDegree;                // Determines the gauge pointer's rotation at maximum value.
    [SerializeField]
    float curGaugeDegree;                       // The gauge pointer's current rotation.

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
    {
        PartCheck();        
    }
	
	void FixedUpdate()
    {
        CalculateGaugeRotation();
	}

    void PartCheck()
    {
        // Health Check
        if (isHealth)
            health = inputObject.GetComponent<HealthScript>();
        else
            health = null;

        // Temperature Check
        if (isTemp)
            temp = inputObject.GetComponent<TemperatureScript>();
        else
            temp = null;
        
        // Engine Check
        if (isEngine)
            engine = inputObject.GetComponent<SmashEngineScript>();
        else
            engine = null;

        // Wing Check
        if (isLeftWing || isRightWing)
            crank = inputObject.GetComponent<TurnCrankScript>();
        else
            crank = null;
    }

    void CalculateGaugeRotation()
    {
        // Health Gauge
        if (isHealth)
            curGaugeDegree = health.gaugeRotation * maxGaugeDegree;

        // Temp Gauge
        if (isTemp)
            curGaugeDegree = temp.gaugeRotation * maxGaugeDegree;

        // Engine Gauge
        if (isEngine)
            curGaugeDegree = engine.gaugeRotation * maxGaugeDegree;

        // Wing Gauge
        if (isLeftWing || isRightWing)
            curGaugeDegree = crank.gaugeRotation * maxGaugeDegree;

        transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, curGaugeDegree));
    }
}
