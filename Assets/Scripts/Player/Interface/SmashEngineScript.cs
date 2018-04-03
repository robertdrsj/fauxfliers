using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class SmashEngineScript : MonoBehaviour {

    // Initialize
    PlayerController player;
    AirplaneController airplane;

    // Operation
    public bool isWorking;                  // Flag if the engine is working fine.

    public float minDurability;             // Sets the min amount of durability.
    public float maxDurability;             // Sets the max amount of durability.
    public float curDurability;             // DO NOT EDIT. Indicates how much durability the engine has.

    public float decayAmount;               // How much the durability decays while flying.
    public float regenAmount;               // How much the durability recovers when not providing input.
    public float repairAmount;                     // How much the durability is repaired when smashed once. Value is set in the UI Button.

    public float tempBuffer;                // Can either lessen or worsen the effects of temperature. Initially set at a value of 1.
    public float gaugeRotation;             // Used to determine the gauge's rotation.

    // Repair
    public Button engineButton;
    public bool engineInteractable;         // Flag if the engine can be interacted with.

    public int minSmashFreq;                // The min possible amount of times the engine must be smashed to be fixed.
    public int maxSmashFreq;                // The max possible amount of times the engine must be smashed to be fixed.
    public int smashGoalAmount;             // The randomly set amount of times the engine msut be smashed, based on the min and max frequencies.
    public int smashGoalCurrent;            // The amount of times left for the engine to be smashed enough times to be fixed.

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        airplane = FindObjectOfType<AirplaneController>();
        isWorking = true;
        SetRandomGoal();
    }
	
	void FixedUpdate()
    {
        EngineCheck();
        ManageDurability();
	}

    // Engine Flags
    void EngineCheck()
    {
        if (!isWorking)
        {
            airplane.engineOperable = false;
            engineInteractable = true;
            engineButton.enabled = true;
        }
        else
        {
            airplane.engineOperable = true;
            engineInteractable = false;
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
            isWorking = false;
            curDurability = minDurability;
            CameraShaker.Instance.ShakeOnce(15f, 15f, .1f, 1f);
        }
    }

    // Uses durability when the player is flying the plane.
    void UseDurability(float engineDecayValue)
    {
        if (curDurability >= minDurability)
            curDurability -= (engineDecayValue * (airplane.currentTemp * tempBuffer)) * Time.deltaTime;
    }

    // Regenerates durability when the engine is working but the player isn't flying.
    void RegenDurability(float engineRegenValue)
    {
        if (curDurability <= maxDurability)
            curDurability += (engineRegenValue - (airplane.currentTemp * tempBuffer)) * Time.deltaTime;
    }

    // Repairs durability on engine smash button press.
    public void RepairDurability(float engineRepairValue)
    {
        // If the engine is fully repaired, set durability to max and flag as operable.
        if (curDurability >= maxDurability)
        {
            curDurability = maxDurability;
            isWorking = true;
        }
        // --Otherwise, repair the engine.
        else
        {
            curDurability += engineRepairValue;
            CameraShaker.Instance.ShakeOnce(5f, 5f, .1f, .3f);
        }
    }

    // Randomly sets how many times the player needs to smash the engine for the engine to be fixed.
    void SetRandomGoal()
    {
        int randomSmashFreq = Random.Range(minSmashFreq, maxSmashFreq);
        smashGoalAmount = randomSmashFreq;
        smashGoalCurrent = smashGoalAmount;
    }

    // Deducts from the current Smash Goal. Executes on button press.
    public void DeductFromSmashGoal(int reductionValue)
    {
        smashGoalCurrent -= reductionValue;

        if (smashGoalCurrent <= 0)
        {
            SetRandomGoal();
            engineInteractable = false;
        }
    }

    // Provides the engine gauge with a rotation value between 0 and 1.
    void GaugeValue()
    {
        if (isWorking)
            gaugeRotation = curDurability / maxDurability;
        else
            gaugeRotation = smashGoalCurrent / smashGoalAmount;
    }
}
