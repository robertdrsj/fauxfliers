using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    // INITIALIZE
    /// Mouse Controls
    public bool lMB;                    // Flag if player presses left mouse button.
    public bool rMB;                    // Flag if player presses right mouse button.
    /// Components
    Rigidbody aircraft;

    // FindMousePosition()
    Vector3 mousePos;
    Vector3 mousePosAbs;
    Vector3 originPos;
    float originToMouse;

    // FindMouseQuadrant()
    bool mousePosXPositive;
    bool mousePosYPositive;
    int quadrant;

    // SetAirplaneAngle()
    public float rotationDamping;
    public float turningRate;
    public float pitchAmp;

    float rollAmount;
    float pitchAmount;
    Quaternion pitchAngle;
    Quaternion rollAngle;
    public Quaternion totalAngle;       // Do Not Edit

    Quaternion currentRotation;
    Quaternion newRotation;
    float autoRotationRate;

    //////////

    void Start()
    {
        aircraft = GetComponent<Rigidbody>();

        newRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        lMB = Input.GetMouseButton(0);
        rMB = Input.GetMouseButton(1);

        FindMousePosition();
        FindMouseQuadrant();

        autoRotationRate = originToMouse / rotationDamping;
        currentRotation = aircraft.rotation;

        if (lMB)
        {
            SetAirplaneAngle(mousePos.x, mousePos.y);
            newRotation = Quaternion.Slerp(aircraft.rotation, totalAngle, turningRate);
            aircraft.rotation = currentRotation * newRotation;
        }
    }

    void FindMousePosition()
    {
        // Find Mouse Position by moving the screen origin from bottom left to the center,
        // Then get the mouse position's distance coordinates away from the origin.

        originPos = Camera.main.ScreenToViewportPoint(new Vector3(
            (Camera.main.scaledPixelWidth / 2f),
            (Camera.main.scaledPixelHeight / 3f),
            Camera.main.nearClipPlane));

        mousePos = Camera.main.ScreenToViewportPoint(new Vector3(
            (Input.mousePosition.x - (Camera.main.scaledPixelWidth / 2f)),
            (Input.mousePosition.y - (Camera.main.scaledPixelHeight / 3f)),
            Camera.main.nearClipPlane));

        mousePosAbs = Camera.main.ScreenToViewportPoint(new Vector3(
            Mathf.Abs(Input.mousePosition.x - (Camera.main.scaledPixelWidth / 2f)),
            Mathf.Abs(Input.mousePosition.y - (Camera.main.scaledPixelHeight / 3f)),
            Camera.main.nearClipPlane));

        originToMouse = Mathf.Sqrt(Mathf.Pow(mousePos.x, 2) + Mathf.Pow(mousePos.y, 2));
    }

    void FindMouseQuadrant()
    {
        // Find which quadrant the mouse cursor is in and whether mousePos X and Y are positive or negative.

        if (Mathf.Sign(mousePos.x) == 1) mousePosXPositive = true;
        else mousePosXPositive = false;

        if (Mathf.Sign(mousePos.y) == 1) mousePosYPositive = true;
        else mousePosYPositive = false;

        if (mousePosXPositive && mousePosYPositive)     quadrant = 1;
        if (!mousePosXPositive && mousePosYPositive)    quadrant = 2;
        if (!mousePosXPositive && !mousePosYPositive)   quadrant = 3;
        if (mousePosXPositive && !mousePosYPositive)    quadrant = 4;
    }

    void SetAirplaneAngle(float xDist, float yDist)
    {
        // Adjust plane roll (z-axis)
        rollAmount = Mathf.Atan2(yDist, xDist) * Mathf.Rad2Deg;

        if (quadrant == 1 || quadrant == 2)
            rollAngle = Quaternion.AngleAxis((rollAmount - 90) * autoRotationRate, Vector3.forward);
        if (quadrant == 3 || quadrant == 4)
            rollAngle = Quaternion.AngleAxis((rollAmount + 90) * autoRotationRate, Vector3.back);

        // Adjust plane pitch (x-axis)
        pitchAmount = mousePosAbs.y * pitchAmp;

        if (quadrant == 1 || quadrant == 2)
            pitchAngle = Quaternion.AngleAxis(pitchAmount * autoRotationRate, Vector3.left);
        if (quadrant == 3 || quadrant == 4)
            pitchAngle = Quaternion.AngleAxis(pitchAmount * autoRotationRate, Vector3.right);
        ///Make the pitch adjustment not as harsh when going from positive to negative, and vice versa.
        if ((mousePosAbs.y < 0.1f && mousePosAbs.y > 0.1f) && mousePosAbs.x > 0.3f)
            pitchAngle = Quaternion.AngleAxis(pitchAmount * 1000f * autoRotationRate, Vector3.right);

        // Generate total plane quaternion angle.
        totalAngle = pitchAngle * rollAngle;
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
