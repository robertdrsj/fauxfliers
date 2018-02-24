using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneController : MonoBehaviour {

    // Fly()
    public float thrustForce;                   // Determines how much AddForce to apply for acceleration.
    public float maxVelocity;                   // Determines the max velocity of the aircraft.
    public float maxDrag;                       // Used to decelerate the aircraft.
    public float minDrag;                       // Used to accelerate the aircraft.
    public float noDrag;

    // ManageGravity()
    public GameObject com;                      // Used to find the Center of Mass for flight and gravity.

    public Vector3 minGravity;                  // Used to give the player more flight control.
    public Vector3 maxGravity;                  // Used when the player is giving no flight input (freefall).

    public float angularVelocityThreshold;      // Determines how much to decelerate the aircraft if the player is rotating a lot.

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

    float pitchDir;                     // Rotates around X-axis.
    float yawDir;                       // Rotates around Y-axis.
    float rollDir;                      // Rotates around Z-axis.

    [SerializeField]
    bool allPartsOperable;              // All three parts are usable.
    bool leftWingOperable;              // Left thrust is usable.
    bool rightWingOperable;             // Right thrust is usable.
    bool engineOperable;                // Center thrust is usable.

    public bool canFly;                 // DO NOT EDIT. If Engine Durability is fine, thrusters can be used.
    public bool isFlying;               // DO NOT EDIT. If there's RMB or touch input within the level, allow the player to fly forward.

    bool canRepair;                     // If there's LMB or touch input on engine UI, allow the player to repair engine parts.
    bool repairLeftWing;                // Flag true is the left thruster is currently being repaired.
    bool repairRightWing;               // Flag true if the right thruster is currently being repaired.
    bool repairEngine;                  // Flag true if the center thruster/engine is currently being repaired.

    // INITIALIZE
    PlayerController player;
    Rigidbody aircraft;



	void Start()
    {
        player = GetComponent<PlayerController>();
        aircraft = GetComponent<Rigidbody>();

        canFly = true;
        allPartsOperable = true;
	}

    void FixedUpdate()
    {
        thrustForward = aircraft.transform.forward;

        ManageGravity();

        // Allows flight.
        if (canFly && player.lMB)
            isFlying = true;
        else
        {
            isFlying = false;
            aircraft.drag = noDrag;
        }

        // Checks if the aircraft has any broken parts.
        if (isFlying && allPartsOperable)
        {
            // If the player is tumbling while in full control, increase air drag to decelerate.
            if ((Mathf.Abs(player.totalAngle.x) >= angularVelocityThreshold))
                aircraft.drag = maxDrag;
            else
                aircraft.drag = minDrag;

            Fly();
        }
    }

    void ManageGravity()
    {
        // Simulate center of mass rotation.
        aircraft.centerOfMass = com.transform.position;

        // Change gravity values so the player feels like they have more control when the airplane works fine.
        if (canFly && allPartsOperable)
            Physics.gravity = minGravity;
        else
            Physics.gravity = maxGravity;
    }

    void Fly()
    {
            aircraft.AddForce(thrustForward * thrustForce);
            aircraft.velocity = Vector3.ClampMagnitude(aircraft.velocity, maxVelocity);
    }




}
