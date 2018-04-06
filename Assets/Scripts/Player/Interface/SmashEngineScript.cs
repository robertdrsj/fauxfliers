using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class SmashEngineScript : MonoBehaviour {

    // Initialize
    AirplaneController airplane;
    TemperatureScript temp;

    // Operation
    //[HideInInspector]
    public bool isWorking;                  // Flag if the engine is working fine.

    public float minDurability;             // Sets the min amount of durability.
    public float maxDurability;             // Sets the max amount of durability.
    [SerializeField]
    float curDurability;                    // Indicates how much durability the engine has.

    public float decayAmount;               // How much the durability decays while flying.
    public float regenAmount;               // How much the durability recovers when not providing input.
    float repairAmount;                     // How much the durability is repaired when smashed once.

    public float tempBuffer;                // Can either lessen or worsen the effects of temperature. Initially set at a value of 1.
    [HideInInspector]
    public float gaugeRotation;             // DO NOT EDIT. Used to determine the gauge's rotation.

    // Repair
    public Button engineButton;

    public int minRepairValue;              // The min possible amount of times the engine must be smashed to be fixed.
    public int maxRepairValue;              // The max possible amount of times the engine must be smashed to be fixed.

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        airplane = FindObjectOfType<AirplaneController>();
        temp = FindObjectOfType<TemperatureScript>();
        isWorking = true;
    }
	
	void FixedUpdate()
    {
        if (airplane.enableBreakage)
        {
            EngineCheck();
            ManageDurability();
            GaugeCheck();
        }
        else
            EngineCheck();
	}

    // Engine Flags
    void EngineCheck()
    {
        if (!isWorking)
        {
            airplane.engineOperable = false;
            engineButton.enabled = true;
        }
        else
        {
            airplane.engineOperable = true;
            engineButton.enabled = false;
        }
    }

    // Manages engine durability while flying, including Use/Regen/Repair.
    void ManageDurability()
    {
        // Determines whether the airplane uses or regenerates durability.
        if (isWorking)
        {
            if (airplane.isFlying)
                UseDurability(decayAmount);
            else
                RegenDurability(regenAmount);
        }

        // If durability falls below minimum, break the engine.
        if (curDurability <= minDurability)
        {
            if (isWorking)
            {
                CameraShaker.Instance.ShakeOnce(15f, 15f, .1f, 1f);
                isWorking = false;
            }

            curDurability = minDurability;
        }
    }

    // Uses durability when the player is flying the plane.
    void UseDurability(float engineDecayValue)
    {
        if (curDurability >= minDurability)
            curDurability -= (engineDecayValue + (temp.currentTemp * tempBuffer)) * Time.deltaTime;
    }

    // Regenerates durability when the engine is working but the player isn't flying.
    void RegenDurability(float engineRegenValue)
    {
        if (curDurability <= maxDurability)
            curDurability += (engineRegenValue - (temp.currentTemp * tempBuffer)) * Time.deltaTime;
    }

    // Repairs durability on engine smash button press.
    public void RepairDurability()
    {
        // Sets a random repair value.
        int randomRepairValue = Random.Range(minRepairValue, maxRepairValue);
        repairAmount = randomRepairValue;
        curDurability += repairAmount;
        CameraShaker.Instance.ShakeOnce(5f, 5f, .1f, .3f);

        // If the engine is fully repaired, set durability to max and flag as operable.
        if (curDurability >= maxDurability)
        {
            curDurability = maxDurability;
            isWorking = true;
        }
    }

    // Provides the engine gauge with a rotation value between 0 and 1.
    void GaugeCheck()
    {
        gaugeRotation = curDurability / maxDurability;
    }
}
