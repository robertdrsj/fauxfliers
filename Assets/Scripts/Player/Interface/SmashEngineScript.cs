using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmashEngineScript : MonoBehaviour {

    // Initialize
    PlayerController player;
    AirplaneController airplane;

    // Operation
    public bool isWorking;                  // Flag if the engine is working fine.
    public float decayAmount;               // How much the durability decays while flying.
    public float regenAmount;               // How much the durability recovers when not providing input.
    public float repairAmount;              // How much the durability is repaired when smashed once.

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
    }
	
	void FixedUpdate()
    {
        //EngineCheck();
        engineInteractable = true;

        // Temporary til enginecheck is enabled.
        if (engineInteractable)
        {
            engineButton.enabled = true;
        }
        else
            engineButton.enabled = false;
	}

    // Engine Check
    void EngineCheck()
    {
        if (!airplane.engineOperable)
        {
            engineInteractable = true;
            engineButton.enabled = true;
        }
        else
        {
            engineInteractable = false;
            engineButton.enabled = false;
        }
    }

    // Manages engine durability while flying.
    void ManageDurability()
    {
        if (isWorking)
        {
            airplane.engineOperable = true;
        }
        else
        {
            airplane.engineOperable = false;
        }
    }

    void RepairDurability(float engineRepairValue)
    {

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
}
