using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneController : MonoBehaviour {

    // PUBLIC Variables
    public float thrustForce;

    // PRIVATE Variables
    Vector3 thrustDirection;
    float pitchDir;                     // Rotates around X-axis.
    float yawDir;                       // Rotates around Y-axis.
    float rollDir;                      // Rotates around Z-axis.

    [SerializeField]
    bool allPartsOperable;              // All three parts are usable.
    bool leftWingOperable;              // Left thrust is usable.
    bool rightWingOperable;             // Right thrust is usable.
    bool engineOperable;                // Center thrust is usable.

    [SerializeField]
    bool canFly;                        // If Engine Durability is fine, thrusters can be used.
    [SerializeField]
    bool isFlying;                      // If there's RMB or touch input within the level, allow the player to fly forward.

    bool canRepair;                     // If there's LMB or touch input on engine UI, allow the player to repair engine parts.
    bool repairLeftWing;                // Flag true is the left thruster is currently being repaired.
    bool repairRightWing;               // Flag true if the right thruster is currently being repaired.
    bool repairEngine;                  // Flag true if the center thruster/engine is currently being repaired.

    // INITIALIZE
    PlayerController player;
    Rigidbody airplane;



	void Start()
    {
        player = GetComponent<PlayerController>();
        airplane = GetComponent<Rigidbody>();

        pitchDir = 1f;
        yawDir = 1f;
        rollDir = 1f;

        canFly = true;
        allPartsOperable = true;
	}

    void Update()
    {
        thrustDirection = new Vector3(pitchDir, yawDir, rollDir);

        if (canFly && player.rMB) { isFlying = true; }
            else { isFlying = false; }
    }

    void FixedUpdate()
    {
        if (isFlying)
        {
            if (allPartsOperable)
            {
                airplane.AddForce(airplane.transform.forward * thrustForce);
            }

        }
    }
}
