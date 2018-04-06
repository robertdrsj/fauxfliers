using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class TurnCrankScript : MonoBehaviour {

    // Initialize
    PlayerController player;
    AirplaneController airplane;
    TemperatureScript temp;

    // Operation
    //[HideInInspector]
    public bool isWorking;                  // Flag if the engine is working fine.
    public bool isLeftCrank;                // Flag if the crank controls the left wing.
    public bool isRightCrank;               // Flag if the crank controls the right wing.

    Vector3 crankPos;                       // Provides the crank's position on the screen.
    Vector3 mousePos;                       // Provides the mouse pointer's position on the screen.
    float crankToMouseDis;                  // Provides the distance between the crank and the mouse pointer.
    public float distanceLimit;             // The max distance possible for the mouse to provide input to the crank.
    float crankToMouseRot;                  // The angle at which the mouse is rotating the crank.

    Quaternion lastRot;                     // The crank's last z rotation.
    Quaternion currentRot;                  // The crank's current z rotation.
    float rotationDiff;                     // The different in rotation between lastRot and currentRot.

    public float zDegreesPerSecond;         // Determines how fast the crank should automatically rotate.

    // Repair
    float xMouseDistance;                   // Player mouse coordinates used for durability decay.
    float yMouseDistance;                   // Player mouse coordinates used for durability decay.

    public float minDurability;             // Sets the min amount of durability.
    public float maxDurability;             // Sets the max amount of durability.
    [SerializeField]
    float curDurability;                    // Indicates how much durability the wing has.

    float decayLeftAmount;                  // How much the left wing durability decays while flying.
    float decayRightAmount;                 // How much the right wing durability decays while flying.
    public float decayAmount;
    public float regenAmount;               // How much the durability recovers when not providing input.
    public float minRepairValue;            // Minimum possible random repair amount.
    public float maxRepairValue;            // Maximum possible random repair amount.
    float repairAmount;                     // How much the durability is repaired when the crank rotates.

    public float tempBuffer;                // Can either lessen or worsen the effects of temperature. Initially set at a value of 1.
    [HideInInspector]
    public float gaugeRotation;             // Used to determine the gauge's rotation.

    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        airplane = FindObjectOfType<AirplaneController>();
        temp = FindObjectOfType<TemperatureScript>();
        isWorking = true;
    }
	
	void FixedUpdate()
    {
        if (airplane.enableBreakage)
        {
            WingCheck();
            FindMousePosition();
            ManageRotation();
            ManageDurability(xMouseDistance, yMouseDistance);
            GaugeCheck();
        }
        else
            WingCheck();
    }

    // Checks if left wing is broken and can be interacted with.
    void WingCheck()
    {
        // Left Wing
        if (isLeftCrank)
        {
            if (!isWorking)
                airplane.leftWingOperable = false;
            else
            {
                airplane.leftWingOperable = true;
                transform.Rotate(new Vector3(0f, 0f, 0f));
            }
        }

        // Right Wing
        if (isRightCrank)
        {
            if (!isWorking)
                airplane.rightWingOperable = false;
            else
            {
                airplane.rightWingOperable = true;
                transform.Rotate(new Vector3(0f, 0f, 0f));
            }
        }
    }

    // Finds the mouse cursor position based on the crank's origin position.
    void FindMousePosition()
    {
        crankPos = Camera.main.WorldToViewportPoint(transform.position);
        mousePos = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)); // use ratio for screen res

        // The viewport's X is stretched a little further which skews the radius, so it's been buffed up a bit.
        crankToMouseDis = Mathf.Sqrt((Mathf.Pow((crankPos.x - mousePos.x), 2) * 3f) + Mathf.Pow((crankPos.y - mousePos.y), 2)) * 100f;
        if (isLeftCrank)
            crankToMouseRot = (Mathf.Atan2(crankPos.y - mousePos.y, crankPos.x - mousePos.x) * Mathf.Rad2Deg) + 1200f; // add 180 maybe
        if (isRightCrank)
            crankToMouseRot = (Mathf.Atan2(crankPos.y - mousePos.y, crankPos.x - mousePos.x) * Mathf.Rad2Deg) + 60f;

        // Use the mouse location from the Player script to determine wing durability decay.
        xMouseDistance = player.mousePos.x;
        yMouseDistance = player.mousePos.y;
    }

    // Allow manual rotation, automatic rotation, and calculates rotationDiff for RepairDurability().
    void ManageRotation()
    {
        // Left Crank
        if (isLeftCrank && !isWorking)
        {
            lastRot = transform.localRotation;
            ManageLeftRotation();
            currentRot = transform.localRotation;
            rotationDiff = Mathf.Abs(currentRot.z) - Mathf.Abs(lastRot.z);
        }

        // Right Crank
        if (isRightCrank && !isWorking)
        {
            lastRot = transform.localRotation;
            ManageRightRotation();
            currentRot = transform.localRotation;
            rotationDiff = Mathf.Abs(currentRot.z) - Mathf.Abs(lastRot.z);
        }
    }

    // Manages wing durability while flying, including Use/Regen/Repair.
    void ManageDurability(float xDist, float yDist)
    {
        // Determines whether the airplane uses or regenerates durability.
        if (isWorking)
        {
            // Left Wing
            if (isLeftCrank)
            {
                if (airplane.isFlying)
                {
                    if (xDist <= 0)
                        decayLeftAmount = Mathf.Abs(xDist) + Mathf.Abs(yDist);
                    else
                        decayLeftAmount = 0;

                    UseDurability(decayLeftAmount);
                }
                else
                    RegenDurability(regenAmount);
            }

            // Right Wing
            if (isRightCrank)
            {
                if (airplane.isFlying)
                {
                    if (xDist >= 0)
                        decayRightAmount = Mathf.Abs(xDist) + Mathf.Abs(yDist);
                    else
                        decayRightAmount = 0;

                    UseDurability(decayRightAmount);
                }
                else
                    RegenDurability(regenAmount);
            }

            // If durability falls below minimum, break the wing.
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
    }

    // Uses durability when the player is flying the plane.
    void UseDurability(float decayValue)
    {
        if (curDurability >= minDurability)
            curDurability -= ((decayValue + regenAmount) + (temp.currentTemp * tempBuffer)) * Time.deltaTime;
    }

    // Regenerates durability when the wing is working but the player isn't providing flying input.
    void RegenDurability(float regenValue)
    {
        if (curDurability <= maxDurability)
            curDurability += ((regenValue + regenAmount) - (temp.currentTemp * tempBuffer)) * Time.deltaTime;
    }

    // Keeps track of how far the player has turned the crank.
    void ManageLeftRotation()
    {
        if ((crankToMouseDis <= distanceLimit) && (player.lMB && player.doNotInput))
        {
                transform.localRotation = Quaternion.Euler(0f, 0f, crankToMouseRot);
                RepairDurability();
        }
        else
            AutoRotate(zDegreesPerSecond);

        // Penalize player if they try to cheat the system by spamming left click instead of holding down LMB on crank.
        if (Input.GetMouseButtonUp(0) && !isWorking)
        {
            CameraShaker.Instance.ShakeOnce(3f, 3f, .1f, 1f);
            curDurability -= maxRepairValue * 2f;

            if (curDurability < minDurability)
                curDurability = minDurability;
        }
    }

    void ManageRightRotation()
    {
        // Repair durability when LMB is held down and moues cursor is on the crank.
        if ((crankToMouseDis <= distanceLimit) && (player.lMB && player.doNotInput))
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, -crankToMouseRot);
            RepairDurability();
        }
        else
            AutoRotate(zDegreesPerSecond);

        // Penalize player if they try to cheat the system by spamming left click instead of holding down LMB on crank.
        if (Input.GetMouseButtonUp(0) && !isWorking)
        {
            CameraShaker.Instance.ShakeOnce(3f, 3f, .1f, 1f);
            curDurability -= maxRepairValue * 2f;

            if (curDurability < minDurability)
                curDurability = minDurability;
        }
    }
    
    // Automatically rotates the crank when the wing is broken but not being interacted with.
    void AutoRotate(float rotationRate)
    {
        transform.Rotate(Vector3.forward, rotationRate * Time.deltaTime, Space.Self);
    }

    // Repairs durability on crank rotation.
    void RepairDurability()
    {
        // Sets a random repair value.
        float randomRepairValue = Random.Range(minRepairValue, maxRepairValue);
        repairAmount = randomRepairValue;

        // Repair wing durability based on rotation.
        // Deducts from the current rotation goal, even if its CW or CCW (bc even if you go CCW, the degrees are messed up so it'll think you're CW sometimes).
        if (Mathf.Sign(rotationDiff) == 1)
            curDurability += rotationDiff * repairAmount;
        if (Mathf.Sign(rotationDiff) == -1)
            curDurability += (-rotationDiff) * repairAmount;

        // If the wing is fully repaired, set durability to max and flag as operable.
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
