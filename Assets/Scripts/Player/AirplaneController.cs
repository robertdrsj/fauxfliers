using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneController : MonoBehaviour {

    // BREAKAGE
    // PUBLIC Variables
    public Text healthText;
    public float health;
    public float maxHealth;
    public float currentHealth;

    public Text engineDurText;
    public float engineDur;
    public float engineMaxDur;
    public float engineCurrentDur;

    public Text leftDurText;
    public float leftDur;
    public float leftMaxDur;
    public float leftCurrentDur;

    public Text rightDurText;
    public float rightDur;
    public float rightMaxDur;
    public float rightCurrentDur;

    float decayEngine;
    float decayLeft;
    float decayRight;
    float addDecay;

    // PRIVATE Variables
    Quaternion thrustDirection;
    Quaternion currentDir;
    Quaternion targetDir;
    Quaternion targetDirX;
    Quaternion targetDirY;
    Quaternion targetDirZ;

    Vector3 thrustVertical;
    Vector3 thrustLeft;
    Vector3 thrustRight;
    Vector3 thrustNull;

    public bool allPartsOperable;              // All three parts are usable.
    bool leftWingOperable;              // Left thrust is usable.
    bool rightWingOperable;             // Right thrust is usable.
    bool engineOperable;                // Center thrust is usable.

    public bool isFlying;               // DO NOT EDIT. If there's RMB or touch input within the level, allow the player to fly forward.

    bool canRepair;                     // If there's LMB or touch input on engine UI, allow the player to repair engine parts.
    bool repairLeftWing;                // Flag true is the left thruster is currently being repaired.
    bool repairRightWing;               // Flag true if the right thruster is currently being repaired.
    bool repairEngine;                  // Flag true if the center thruster/engine is currently being repaired.

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

    Vector3 thrustForward;
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

        allPartsOperable = true;
	}

    void FixedUpdate()
    {
        thrustForward = aircraft.transform.forward;

        ManageGravity();

        // Allows flight.
        if (allPartsOperable)
        {
            aircraft.drag = maxDrag;

            if (player.lMB)     isFlying = true;
            else                isFlying = false;
        }
        else
        {
            isFlying = false;
            aircraft.drag = noDrag;
        }

        // Fly via player input.
        if (isFlying)
        {
            Fly();
        }
    }

    void ManageGravity()
    {
        // Simulate center of mass rotation.
        aircraft.centerOfMass = com.transform.position;

        // Change gravity values so the player feels like they have more control when the airplane works fine.
        if (allPartsOperable)
            Physics.gravity = minGravity;
        else
            Physics.gravity = maxGravity;
    }

    void Fly()
    {
            aircraft.AddForce(thrustForward * thrustForce);
            aircraft.velocity = Vector3.ClampMagnitude(aircraft.velocity, maxVelocity);
    }
    
    void UseEngineDurability(float decayValue)
    {
        if (engineCurrentDur > 0)
            engineCurrentDur -= decayValue * Time.deltaTime;
        else
        {
            allPartsOperable = false;
            //screenshake.shakeAmount = 0.5f;
            //screenshake.shakeDuration = 0.3f;
            //engineExplode.Play();
        }
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
