using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class AirplaneController : MonoBehaviour {

    // Initialize
    PlayerController player;
    Rigidbody aircraft;
    HealthScript health;
    TemperatureScript temp;

    // Setup
    public GameObject engine;
    public GameObject leftWing;
    public GameObject rightWing;

    public bool enableHealth;                   // Enable/disable health.
    public bool enableTemp;                     // Enable/disable temperature.
    public bool enableBreakage;                 // Enable/disable breakage.

    // Operation
    [HideInInspector]
    public bool allPartsOperable;               // All three parts are usable.
    [HideInInspector]
    public bool engineOperable;                 // Engine is usable.
    [HideInInspector]
    public bool leftWingOperable;               // Left wing is usable.
    [HideInInspector]
    public bool rightWingOperable;              // Right wing is usable.

    [HideInInspector]
    public bool isFlying;                       // Flag if there's flying input and all parts are operable.
    [HideInInspector]
    public bool collisionDetected;              // Flag if a collision has been detected. Used mainly for HealthScript.
    [SerializeField]
    float rbVelocity;                           // Displays the rigidbody's current velocity.

    // Flight
    Vector3 thrustDirection;
    Vector3 thrustNull;
    Vector3 thrustForward;
    Vector3 thrustLeft;
    Vector3 thrustRight;

    public float thrustForce;                   // Determines how much AddForce to apply for acceleration.
    public float maxDrag;                       // Used to decelerate the aircraft.
    public float minDrag;                       // Used to accelerate the aircraft.
    public float noDrag;                        // Used when the engine breaks and the aircraft is in freefall.

    public GameObject com;                      // Used to find the Center of Mass for flight and gravity.
    public Vector3 minGravity;                  // Used to give the player more flight control.
    public Vector3 maxGravity;                  // Used when the player is giving no flight input (freefall).

    float pitchDir;                             // Rotates around X-axis.
    float yawDir;                               // Rotates around Y-axis.
    float rollDir;                              // Rotates around Z-axis.

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        player = GetComponent<PlayerController>();
        aircraft = GetComponent<Rigidbody>();
        health = GetComponent<HealthScript>();
        temp = GetComponent<TemperatureScript>();

        engineOperable = true;
        leftWingOperable = true;
        rightWingOperable = true;
        allPartsOperable = true;
    }

    void FixedUpdate()
    {
        OperationCheck();
        ManageDrag();
        ManageGravity();
        ManageThrust();
        ManageFlight();

        rbVelocity = aircraft.velocity.magnitude;
    }

    // Checks the status of all interactive airplane parts.
    void OperationCheck()
    {
        if (!engineOperable || !leftWingOperable || !rightWingOperable)
            allPartsOperable = false;
        else
            allPartsOperable = true;
    }

    // Sets rigidbody drag values based on operable parts.
    void ManageDrag()
    {
        if (allPartsOperable && !isFlying)
            aircraft.drag = maxDrag;
        else if ((allPartsOperable && isFlying) || engineOperable)
            aircraft.drag = minDrag;
        else
            aircraft.drag = noDrag;
    }

    // Sets project gravity based on whether the engine breaks or not.
    void ManageGravity()
    {
        if (engineOperable)
            Physics.gravity = minGravity;
        else
            Physics.gravity = maxGravity;
    }

    // Sets a mode of thrust when a part stops functioning.
    void ManageThrust()
    {
        thrustNull = new Vector3(0f, 0f, 0f);
        thrustForward = aircraft.transform.forward;
        thrustLeft = aircraft.transform.up - aircraft.transform.right;
        thrustRight = aircraft.transform.up + aircraft.transform.right;
    }

    // Sets center of mass, isFlying flag, and flying pattern.
    void ManageFlight()
    {
        // Simulate center of mass rotation.
        aircraft.centerOfMass = com.transform.position;

        // Set flags.
        if (engineOperable)
        {
            if (player.lMB && !player.doNotInput)
                isFlying = true;
            else
                isFlying = false;
        }
        else
            isFlying = false;

        // Sets flight pattern.
        if (allPartsOperable)
        {
            if (isFlying)
                ManualFlight();
            else
                AutopilotFlight();
        }
        else
        {
            if (((!leftWingOperable && rightWingOperable) || (!rightWingOperable && leftWingOperable)) && engineOperable)
                AutopilotFlight();

            if (!engineOperable || (engineOperable && !leftWingOperable && !rightWingOperable))
                DownwardFlight();
        }
    }

    // Runs thrust physics for input-based flight.
    void ManualFlight()
    {
        aircraft.AddForce(thrustForward * thrustForce);
    }

    // Runs thrust physics for no-input flight.
    void AutopilotFlight()
    {
        aircraft.AddForce(thrustForward * (thrustForce / 2f));
    }

    // Controls downward flight when the engine -- or both wings -- are inoperable.
    void DownwardFlight()
    {
        /// NOTE: There is no statement for !engineOperable since gravity does the acceleration work if the engine breaks.

        if (engineOperable && !leftWingOperable && !rightWingOperable)
            if (player.lMB && !player.doNotInput)
                aircraft.AddForce(thrustForward * (thrustForce / 4f));
    }
    








    // OLD CODE:

    /*
    // Public variables
    public static PlayerController instance = null;
    public AudioSource engineRev;
    public AudioSource engineExplode;
    public AudioSource engineMash;
    public AudioSource healUp;
    public AudioSource wallCollide;
    public bool isThrusting;
    public bool isSmoking;

    /// Airplane Stats ///
    public float thrust;                    // The amount of force exerted onto airplane.
    public bool isBroken;                   // Flags whether the airplane can be flown. If durability is zero, isBroken is true. Do NOT touch this in inspector! Viewing only.
    public bool isLowHealth;                // Flags whether the player has less than 10% health.
    public Stat durability;                 // Engine's current durability.
    public float decayAmt;                  // How much durability is lost.
    public float regenAmt;                  // How much durability is regenerated.
    public float repairAmt;                 // How much the engine's durability increases for each button press to repair.
    public float healthRepairAmt;           // How much health is regained when repairing.
    public float shootDecayAmt;             // How much the durability is lost while shooting.

    public bool isDead;                     // Flags whether the airplane exploded. If health is zero, isDead is true. Do NOT touch this in inspector also.
    public bool isHurt;                     // Flags whether the player just took damage.
    public Stat health;                     // Airplane's current health.
    public int wallCollisionDamage;

    // Private variables
    Rigidbody2D player;
    ExplosionController explosion;
    ScreenshakeController screenshake;
    AltimeterController altitude;
    GameObject gameOver;

    // Serialized variables
    [SerializeField]
    CanvasGroup stageSuccess;
    
    /// Set Airplane Angle ///
    Vector3 mousePos;
    Vector3 planeToMouseDir;
    float airplaneAngle;

    /////////////////////
    // Player Movement //
    /////////////////////

    void Awake()
    {
        // ensures that PlayerController is a singleton
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        // Initialize Stats
        durability.Initialize();
        health.Initialize();
    }

    void Start ()
    {
        player = GetComponent<Rigidbody2D>();
        explosion = GetComponent<ExplosionController>();
        screenshake = FindObjectOfType<ScreenshakeController>();
        altitude = FindObjectOfType<AltimeterController>();

        engineRev = GetComponentInChildren<AudioSource>();
        engineExplode = GetComponentInChildren<AudioSource>();
        engineMash = GetComponentInChildren<AudioSource>();
        healUp = GetComponentInChildren<AudioSource>();
        wallCollide = GetComponentInChildren<AudioSource>();

        stageSuccess.GetComponent<CanvasGroup>().alpha = 0.0f;

        isBroken = false;
        isHurt = false;
	}
	
	void Update ()
    {
        SetAirplaneAngle();                                 // Always update airplane's angle to be directed towards the mouse cursor.
        SmokeToggle();
        BarAnimation();

        // RMB creates Thrust as long as engine is not broken.
        if (!isBroken && Input.GetMouseButton(1))
        {
            Thrust();
            UseDurability();
        }
        else if (!isBroken && Input.GetMouseButton(0))
            GunUsesDurability();
        else
            RepairEngine();

        if (!isBroken && Input.GetMouseButtonDown(1))
            engineRev.Play();

        if (health.CurrentVal <= 0f)
            DieSpectacularly();

        if (altitude.timeCurrent >= altitude.timeMax)
        {
            WinGame();
        }
    }

    void TakeDamageFor(float damageTaken)
    {
        health.CurrentVal -= damageTaken;
    }

    void HealFor(float healthRestored)
    {
        health.CurrentVal += healthRestored;
    }

    void SmokeToggle()
    {
        if (Input.GetMouseButtonDown(1) && !isBroken)
            isThrusting = true;
        if (Input.GetMouseButtonUp(1))
            isThrusting = false;

        if (isBroken || durability.CurrentVal <= 20f)
            isSmoking = true;
        else
            isSmoking = false;
    }

    void DieSpectacularly()
    {
        screenshake.shakeAmount = 0.8f;
        screenshake.shakeDuration = 0.5f;
        Destroy(gameObject);
        explosion.Animate();
        gameOver = GameObject.Find("GameOverScreen");
        CanvasGroup UI = gameOver.GetComponent<CanvasGroup>();
        UI.alpha = 1;
    }

    void WinGame()
    {
        health.CurrentVal = health.MaxVal;
        stageSuccess.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            health.CurrentVal -= wallCollisionDamage;
            screenshake.shakeAmount = 0.2f;
            screenshake.shakeDuration = 0.1f;
        }
    }

    */
}
