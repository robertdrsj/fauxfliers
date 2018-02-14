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

    void Update()
    {
        // Used for debugging.
        velocity = airplane.velocity;

        totalAngleX = player.totalAngle.x;
        totalAngleY = player.totalAngle.y;
        totalAngleZ = player.totalAngle.z;

        // Allows flight.
        if (canFly && player.lMB) { isFlying = true; }
            else { isFlying = false; }

    }

    void FixedUpdate()
    {
        CalibrateThrusters();

        if (isFlying && allPartsOperable)
            Fly();
    }

    void CalibrateThrusters()
    {
        thrustForward = airplane.transform.forward;
        thrustVertical = airplane.transform.right / 2;
        thrustNull = new Vector3(0f, 0f, 0f);
    }

    void Fly()
    {
        currentDir = airplane.transform.rotation;

        // Set thrustUpward
        if (player.lMB)
        {
            if (player.originToMouse == 0)
                pitchDir = 0f;
            if (player.originToMouse > 0 && player.totalAngle.x > 0)
                pitchDir = player.originToMouse * -thrustAngleAmp;
            if (player.originToMouse > 0 && player.totalAngle.x < 0)
                pitchDir = player.originToMouse * thrustAngleAmp;

            if (player.originToMouse > 0 && player.totalAngle.z > 0)
                rollDir = player.originToMouse * (thrustAngleAmp * thrustZPower);
            if (player.originToMouse > 0 && player.totalAngle.z < 0)
                rollDir = player.originToMouse * -(thrustAngleAmp * thrustZPower);
        }

        targetDirX = Quaternion.AngleAxis(pitchDir, Vector3.left);
        targetDirY = Quaternion.AngleAxis(yawDir, Vector3.down);
        targetDirZ = Quaternion.AngleAxis(rollDir, Vector3.forward);
        targetDir = targetDirX * targetDirY * targetDirZ;

        //Quaternion.RotateTowards();

        airplane.AddForce(targetDir * thrustForward * thrustForce);

        Debug.Log(thrustVertical);

    }
}
