using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneController : MonoBehaviour {

    // PUBLIC Variables
    public float thrustForce;
    [SerializeField]
    Vector3 velocity;

    // PRIVATE Variables
    Quaternion thrustDirection;
    Quaternion currentDir;
    Quaternion targetDir;
    Quaternion targetDirX;
    Quaternion targetDirY;
    Quaternion targetDirZ;

    Vector3 thrustForward;
    Vector3 thrustVertical;
    Vector3 thrustLeft;
    Vector3 thrustRight;
    Vector3 thrustNull;

    [SerializeField]
    float totalAngleX;
    [SerializeField]
    float totalAngleY;
    [SerializeField]
    float totalAngleZ;

    public float pitchDir;                     // Rotates around X-axis.
    public float yawDir;                       // Rotates around Y-axis.
    public float rollDir;                      // Rotates around Z-axis.

    public float thrustAngleAmp;        // Adjust this to increase or decrease the angle away from Forward.
    public float thrustZPower;

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

        canFly = true;
        allPartsOperable = true;
	}

    void FixedUpdate()
    {
        // Allows flight.
        if (canFly && player.lMB) { isFlying = true; }
        else { isFlying = false; }

        if (isFlying && allPartsOperable)
            Fly();
    }

    void Fly()
    {
        thrustForward = airplane.transform.forward;

        airplane.AddForce(thrustForward * thrustForce);
    }
}
