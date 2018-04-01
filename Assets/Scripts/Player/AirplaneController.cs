using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class AirplaneController : MonoBehaviour {

    // BREAKAGE
    // PUBLIC Variables
    public Text healthText;
    public float maxHealth;
    public float currentHealth;

    public Text engineDurText;
    public float engineMaxDur;
    public float engineCurrentDur;

    public Text leftDurText;
    public float leftMaxDur;
    public float leftCurrentDur;

    public Text rightDurText;
    public float rightMaxDur;
    public float rightCurrentDur;

    // DURABILITY MANAGEMENT
    public bool enableHealth;
    public bool enableBreakage;
    float xMouseDistance;
    float yMouseDistance;

    public bool allPartsOperable;       // All three parts are usable.
    public bool engineOperable;         // Engine is usable.
    public bool leftWingOperable;       // Left wing is usable.
    public bool rightWingOperable;      // Right wing is usable.

    public bool isFlying;               // DO NOT EDIT. If there's RMB or touch input within the level, allow the player to fly forward.

    public float decayAllAmp;           // Set how much all parts should decay per second.
    public float decayWingAmp;          // Set how much the wings should decay.
    public float decayEngine;           // Set how much the engine should decay.
    float decayLeft;
    float decayRight;

    public float engineRegenAmt;
    public float leftWingRegenAmt;
    public float rightWingRegenAmt;

    public float engineRepairAmt;
    public float leftWingRepairAmt;
    public float rightWingRepairAmt;

    public bool engineRepairCmd;
    public bool leftRepairCmd;
    public bool rightRepairCmd;

    public float brokenThrustAmp;               // Used when a Wing part is broken.
    Vector3 thrustDirection;
    Vector3 thrustNull;
    Vector3 thrustForward;
    Vector3 thrustClockwise;
    Vector3 thrustCounterClockwise;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // STANDARD FLIGHT
    // Fly()
    public float thrustForce;                   // Determines how much AddForce to apply for acceleration.
    public float maxVelocity;                   // Determines the max velocity of the aircraft.
    public float maxDrag;                       // Used to decelerate the aircraft.
    public float minDrag;                       // Used to accelerate the aircraft.
    public float noDrag;                        // Used when the engine breaks and the aircraft is in freefall.

    // ManageGravity()
    public GameObject com;                      // Used to find the Center of Mass for flight and gravity.
    public Vector3 minGravity;                  // Used to give the player more flight control.
    public Vector3 maxGravity;                  // Used when the player is giving no flight input (freefall).
    public float angularVelocityThreshold;      // Determines how much to decelerate the aircraft if the player is rotating a lot.

    float pitchDir;                             // Rotates around X-axis.
    float yawDir;                               // Rotates around Y-axis.
    float rollDir;                              // Rotates around Z-axis.

    // INITIALIZE
    PlayerController player;
    Rigidbody aircraft;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        player = GetComponent<PlayerController>();
        aircraft = GetComponent<Rigidbody>();

        engineOperable = true;
        leftWingOperable = true;
        rightWingOperable = true;
        allPartsOperable = true;
    }

    void FixedUpdate()
    {
        engineRepairCmd = Input.GetKeyDown(KeyCode.S);
        leftRepairCmd = Input.GetKeyDown(KeyCode.A);
        rightRepairCmd = Input.GetKeyDown(KeyCode.D);

        thrustNull = new Vector3(0f, 0f, 0f);
        thrustForward = aircraft.transform.forward;
        thrustClockwise = aircraft.transform.up + aircraft.transform.right;
        thrustCounterClockwise = aircraft.transform.up - aircraft.transform.right;

        // Run airplane stuffs.
        //TextUI();
        ManageFlight();

        if (isFlying && !player.doNotInput) Fly();
        if (enableHealth) ManageHealth();
        if (enableBreakage) ManageDurability(xMouseDistance, yMouseDistance);
    }

    void TextUI()
    {
        healthText.text = "Health: " + currentHealth;
        engineDurText.text = "Engine Durability: " + engineCurrentDur;
        leftDurText.text = "Left Wing Durability: " + leftCurrentDur;
        rightDurText.text = "Right Wing Durability: " + rightCurrentDur;
    }

    void ManageFlight()
    {
        thrustDirection = thrustForward;

        // Simulate center of mass rotation.
        aircraft.centerOfMass = com.transform.position;

        // Change gravity values so the player feels like they have more control when the airplane works fine.
        if (engineOperable)
            Physics.gravity = minGravity;
        else
            Physics.gravity = maxGravity;

        // Allows flight.
        if (allPartsOperable)
        {
            aircraft.drag = maxDrag;

            if (player.lMB) isFlying = true;
            else isFlying = false;
        }
        else if (engineOperable)
        {
            if (player.lMB) isFlying = true;
            else isFlying = false;
            aircraft.drag = noDrag;
        }
        else
        {
            isFlying = false;
            aircraft.drag = noDrag;
        }

        // Helps get distance of input away from origin.
        xMouseDistance = player.mousePos.x;
        yMouseDistance = player.mousePos.y;
    }

    void Fly()
    {
            aircraft.AddForce(thrustDirection * thrustForce);
            aircraft.velocity = Vector3.ClampMagnitude(aircraft.velocity, maxVelocity);
    }
    
    void ManageHealth()
    {

    }

    void ManageDurability(float xDist, float yDist)
    {
        // Parts operation check.
        if (!engineOperable || !leftWingOperable || !rightWingOperable)
            allPartsOperable = false;
        else
            allPartsOperable = true;

        // Set decay values (engineDecay is publicly set).
        if (isFlying)
        {
            if (xDist <= 0)
                decayLeft = Mathf.Abs(xDist) + Mathf.Abs(yDist);
            else
                decayLeft = 0;

            if (xDist >= 0)
                decayRight = Mathf.Abs(xDist) + Mathf.Abs(yDist);
            else
                decayRight = 0;
        }
        
        // Uses durability based on vigor of flight.
        if (isFlying)
        {
            if (engineOperable)     UseEngineDurability(decayEngine);
            if (leftWingOperable)   UseLeftWingDurability(decayLeft);
            if (rightWingOperable)  UseRightWingDurability(decayRight);
        }

        // Repair durability and health. Regain durability if the player isn't giving input. //
        //Engine
        if (!engineOperable)
        {
            if (engineCurrentDur >= engineMaxDur) engineOperable = true;
            if (engineRepairCmd) RepairEngineDurability(engineRepairAmt);
        }
        else if (!player.lMB && engineOperable && engineCurrentDur < engineMaxDur)
            engineCurrentDur += engineRegenAmt;
        else
            thrustDirection = thrustForward;

        //Left Wing
        if (!leftWingOperable && rightWingOperable)
        {
            // Repair
            if (leftCurrentDur >= leftMaxDur) leftWingOperable = true;
            if (leftRepairCmd) RepairLeftWingDurability(leftWingRepairAmt);
        }
        else if (!player.lMB && leftWingOperable && leftCurrentDur < leftMaxDur)
        {
            leftCurrentDur += leftWingRegenAmt;
        }

        //Right Wing
        if (!rightWingOperable && leftWingOperable)
        {
            // Repair
            if (rightCurrentDur >= rightMaxDur) rightWingOperable = true;
            if (rightRepairCmd) RepairRightWingDurability(rightWingRepairAmt);
        }
        else if (!player.lMB && rightWingOperable && rightCurrentDur < rightMaxDur)
        {
            rightCurrentDur += rightWingRegenAmt;
        }

        //Both Wings Broken
        if (!leftWingOperable && !rightWingOperable)
        {
            if (leftRepairCmd) RepairLeftWingDurability(leftWingRepairAmt);
            if (rightRepairCmd) RepairRightWingDurability(rightWingRepairAmt);
        }
    }

    void UseEngineDurability(float decayValue)
    {
        if (engineCurrentDur > 0 && engineOperable)
        {
            engineCurrentDur -= (decayValue + decayAllAmp) * Time.deltaTime;
        }
        else
        {
            engineOperable = false;
            engineCurrentDur = 0;
            CameraShaker.Instance.ShakeOnce(15f, 15f, .1f, 1f);
            //engineExplode.Play();
        }
    }

    void UseLeftWingDurability(float decayValue)
    {
        if (leftCurrentDur > 0 && leftWingOperable)
        {
            leftCurrentDur -= ((decayValue * decayWingAmp) + decayAllAmp) * Time.deltaTime;
        }
        else
        {
            leftWingOperable = false;
            leftCurrentDur = 0;
            CameraShaker.Instance.ShakeOnce(10f, 10f, .1f, 1f);
            //engineExplode.Play();
        }
    }

    void UseRightWingDurability(float decayValue)
    {
        if (rightCurrentDur > 0 && rightWingOperable)
        {
            rightCurrentDur -= ((decayValue * decayWingAmp) + decayAllAmp) * Time.deltaTime;
        }
        else
        {
            rightWingOperable = false;
            rightCurrentDur = 0;
            CameraShaker.Instance.ShakeOnce(10f, 10f, .1f, 1f);
            //engineExplode.Play();
        }
    }

    void RepairEngineDurability(float repairValue)
    {
        CameraShaker.Instance.ShakeOnce(5f, 5f, .1f, .3f);

        if (engineCurrentDur <= engineMaxDur)       engineCurrentDur += repairValue;
        else if (engineCurrentDur < engineMaxDur)   engineCurrentDur = engineMaxDur;
        else                                        engineOperable = true;
    }

    void RepairLeftWingDurability(float repairValue)
    {
        CameraShaker.Instance.ShakeOnce(5f, 5f, .1f, .3f);

        if (leftCurrentDur <= leftMaxDur) leftCurrentDur += repairValue;
        else if (leftCurrentDur < leftMaxDur) leftCurrentDur = leftMaxDur;
        else leftWingOperable = true;
    }

    void RepairRightWingDurability(float repairValue)
    {
        CameraShaker.Instance.ShakeOnce(5f, 5f, .1f, .3f);

        if (rightCurrentDur <= rightMaxDur) rightCurrentDur += repairValue;
        else if (rightCurrentDur < rightMaxDur) rightCurrentDur = rightMaxDur;
        else rightWingOperable = true;
    }

    void HealFor(float healthRestored)
    {
        currentHealth += healthRestored;
    }

    void TakeDamageFor(float damageTaken)
    {
        currentHealth -= damageTaken;
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

    void SetAirplaneAngle()
    {
        mousePos = Camera.main.WorldToScreenPoint(transform.position);
        planeToMouseDir = Input.mousePosition - mousePos;
        airplaneAngle = Mathf.Atan2(planeToMouseDir.y, planeToMouseDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(airplaneAngle - 90, Vector3.forward);
    }

    void TakeDamageFor(float damageTaken)
    {
        health.CurrentVal -= damageTaken;
    }

    void HealFor(float healthRestored)
    {
        health.CurrentVal += healthRestored;
    }

    void Thrust()
    {
        player.AddForce(player.transform.up * thrust);
    }

    void UseDurability()
    {
        if (durability.CurrentVal > 0)
            durability.CurrentVal -= decayAmt * Time.deltaTime;
        else
        {
            isBroken = true;
            screenshake.shakeAmount = 0.5f;
            screenshake.shakeDuration = 0.3f;
            engineExplode.Play();
        }
    }

    void GunUsesDurability()
    {
        if (durability.CurrentVal > 0)
            durability.CurrentVal -= shootDecayAmt * Time.deltaTime;
        else
        {
            isBroken = true;
        }
    }

    void RepairEngine()
    {
        if (isBroken)
        {
            if (Input.GetMouseButtonDown(1))
            {
                durability.CurrentVal += repairAmt;
                HealFor(healthRepairAmt);
                screenshake.shakeAmount = 0.45f;
                screenshake.shakeDuration = 0.1f;
                engineMash.Play();
                healUp.Play();
            }
            if (durability.CurrentVal >= durability.MaxVal)
                isBroken = false;
        }
        else if (!isBroken && durability.CurrentVal < durability.MaxVal)
        {
                durability.CurrentVal += regenAmt * Time.deltaTime;
        }
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

    public void BarAnimation()
    {

        if (health.CurrentVal <= 20f)
            GameObject.Find("HealthBarContainer").GetComponent<Animator>().SetBool("isLowHealth", true);
        else
            GameObject.Find("HealthBarContainer").GetComponent<Animator>().SetBool("isLowHealth", false);
        if (isBroken)
            GameObject.Find("EngineBarContainer").GetComponent<Animator>().SetBool("isBroken", true);
        else
            GameObject.Find("EngineBarContainer").GetComponent<Animator>().SetBool("isBroken", false);
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
