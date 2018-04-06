using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    // INITIALIZE
    /// Mouse Controls
    public bool lMB;                    // Do not edit. Flag if player presses left mouse button.
    public bool rMB;                    // Do not edit. Flag if player presses right mouse button.

    public GameObject dashboard;
    public bool mouseOverDashboard;     // Do not edit. Flag if mouse cursor is hovering over the dashboard UI.
    public bool mouseExitDashboard;     // Do not edit. Flag if mouse cursor left the dashboard UI.
    public bool doNotInput;             // Do not edit. Flag if the player clicks on the dashboard, so they don't keep steering when using the dashboard.

    /// Components
    Rigidbody aircraft;
    AirplaneController airplane;
    CameraController cam;

    // FindMousePosition()
    public Vector3 mousePos;
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
    public float yawAmp;
    public float pitchAmp;

    float rollAmount;
    public float brokenRollAmp;
    Quaternion rollAngle;
    float yawAmount;
    Quaternion yawAngle;
    float pitchAmount;
    Quaternion pitchAngle;
    public Quaternion totalAngle;               // Do Not Edit
    Quaternion fallAngle;                       // Used to calculate angle when falling due to gravity.

    Quaternion currentRotation;
    Quaternion newRotation;
    float autoRotationRate;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        aircraft = GetComponent<Rigidbody>();
        airplane = GetComponent<AirplaneController>();
        cam = FindObjectOfType<CameraController>();

        newRotation = transform.rotation;
    }

    // Inputs Only
    void Update()
    {
        lMB = Input.GetMouseButton(0);
        rMB = Input.GetMouseButton(1);

        // If the player clicked on the dashboard and not the scene, don't generate flying input.
        if (mouseOverDashboard)
        {
            if (Input.GetMouseButtonDown(0))
                doNotInput = true;
        }
        else if (!mouseOverDashboard && mouseExitDashboard && lMB && doNotInput)
            doNotInput = true;
        else
        {
            doNotInput = false;
            mouseExitDashboard = false;
        }
        if (Input.GetMouseButtonUp(0))
            doNotInput = false;
    }

    void FixedUpdate()
    {
        FindMousePosition();
        FindMouseQuadrant();

        autoRotationRate = originToMouse / rotationDamping;
        currentRotation = aircraft.rotation;

        // Used if wings fail
        if (((!airplane.leftWingOperable && airplane.rightWingOperable) || (!airplane.rightWingOperable && airplane.leftWingOperable)) && airplane.engineOperable)
        {
            SetBrokenAngle();
            newRotation = Quaternion.Slerp(aircraft.rotation, totalAngle, turningRate);
            aircraft.rotation = currentRotation * newRotation;
            cam.damping = 12f;
        }

        // Used for standard flight, else if engine fails
        if (lMB && airplane.allPartsOperable && !doNotInput)
        {
            SetAirplaneAngle(mousePos.x, mousePos.y);
            newRotation = Quaternion.Slerp(aircraft.rotation, totalAngle, turningRate);
            aircraft.rotation = currentRotation * newRotation;
            cam.damping = 20f;
        }
        else if (!airplane.engineOperable || (!airplane.leftWingOperable && !airplane.rightWingOperable))
        {
            fallAngle = Quaternion.Euler(aircraft.velocity.normalized.x + 90, aircraft.velocity.normalized.y, aircraft.velocity.normalized.z);
            aircraft.rotation = Quaternion.Slerp(aircraft.rotation, fallAngle, Time.deltaTime);
            cam.damping = 12f;
        }
    }

    void FindMousePosition()
    {
        // Find Mouse Position by moving the screen origin from bottom left to the center,
        // Then get the mouse position's distance coordinates away from the origin.

        mousePos = Camera.main.ScreenToViewportPoint(new Vector3(
            (Input.mousePosition.x - (Camera.main.scaledPixelWidth / 2f)),
            (Input.mousePosition.y - (Camera.main.scaledPixelHeight / 2f)),
            Camera.main.nearClipPlane));

        mousePosAbs = Camera.main.ScreenToViewportPoint(new Vector3(
            Mathf.Abs(Input.mousePosition.x - (Camera.main.scaledPixelWidth / 2f)),
            Mathf.Abs(Input.mousePosition.y - (Camera.main.scaledPixelHeight / 2f)),
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
        // Adjust plane roll (z-axis; roll clockwise/counterclockwise ie. forward/back quaternion)
        rollAmount = Mathf.Atan2(yDist, xDist) * Mathf.Rad2Deg;

        if (airplane.leftWingOperable && airplane.rightWingOperable)
        {
            if (quadrant == 1 || quadrant == 2)
                rollAngle = Quaternion.AngleAxis((rollAmount - 90) * autoRotationRate, Vector3.forward);
            if (quadrant == 3 || quadrant == 4)
                rollAngle = Quaternion.AngleAxis((rollAmount + 90) * autoRotationRate, Vector3.back);
        }

        // Adjust plane yaw (y-axis; turn left/right ie. up/down quaternion)
        yawAmount = mousePosAbs.x * yawAmp;

        if (quadrant == 2 || quadrant == 3)
            yawAngle = Quaternion.AngleAxis(yawAmount * autoRotationRate, Vector3.down);
        if (quadrant == 1 || quadrant == 4)
            yawAngle = Quaternion.AngleAxis(yawAmount * autoRotationRate, Vector3.up);

        // Adjust plane pitch (x-axis; tilt up/down ie. left/right quaternion)
        pitchAmount = mousePosAbs.y * pitchAmp;

        if (quadrant == 1 || quadrant == 2)
            pitchAngle = Quaternion.AngleAxis(pitchAmount * autoRotationRate, Vector3.left);
        if (quadrant == 3 || quadrant == 4)
            pitchAngle = Quaternion.AngleAxis(pitchAmount * autoRotationRate, Vector3.right);
        ///Make the pitch adjustment not as harsh when going from positive to negative, and vice versa.
        if ((mousePosAbs.y < 0.1f && mousePosAbs.y > 0.1f) && mousePosAbs.x > 0.3f)
            pitchAngle = Quaternion.AngleAxis(pitchAmount * 1000f * autoRotationRate, Vector3.right);

        // Generate total plane quaternion angle.
        totalAngle = rollAngle * yawAngle * pitchAngle;
    }

    void SetBrokenAngle()
    {
        // Roll uncontrollably when wings are broken.
        /// Left Wing Broken
        if (!airplane.leftWingOperable && airplane.rightWingOperable && airplane.engineOperable)
            rollAngle = Quaternion.AngleAxis(brokenRollAmp * autoRotationRate, Vector3.forward);
        /// Right Wing Broken
        if (!airplane.rightWingOperable && airplane.leftWingOperable && airplane.engineOperable)
            rollAngle = Quaternion.AngleAxis(brokenRollAmp * autoRotationRate, Vector3.back);

        // Yaw left/right when wings are broken.
        /// Left Wing Broken
        if (!airplane.leftWingOperable && airplane.rightWingOperable && airplane.engineOperable)
            yawAngle = Quaternion.AngleAxis(0f, Vector3.down);
        /// Right Wing Broken
        if (!airplane.rightWingOperable && airplane.leftWingOperable && airplane.engineOperable)
            yawAngle = Quaternion.AngleAxis(0f, Vector3.up);

        // Pitch down when a wing is broken.
        pitchAngle = Quaternion.AngleAxis(0.5f, Vector3.left);

        totalAngle = rollAngle * yawAngle * pitchAngle;
    }
}
