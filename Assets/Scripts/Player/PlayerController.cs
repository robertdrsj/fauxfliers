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

    void FixedUpdate()
    {
        lMB = Input.GetMouseButton(0);
        rMB = Input.GetMouseButton(1);

        FindMousePosition();
        FindMouseQuadrant();

        autoRotationRate = originToMouse / rotationDamping;
        currentRotation = aircraft.rotation;

        fallAngle = Quaternion.Euler(aircraft.velocity.normalized.x + 90, aircraft.velocity.normalized.y, aircraft.velocity.normalized.z);

        // Used if wings fail
        if (!airplane.leftWingOperable && airplane.rightWingOperable && airplane.engineOperable)
        {
            rollAngle = Quaternion.AngleAxis((rollAmount - 90) * Time.deltaTime, Vector3.forward);
            newRotation = Quaternion.Slerp(aircraft.rotation, totalAngle, turningRate * 50f);
            aircraft.rotation = currentRotation * newRotation;
            cam.damping = 5f;
        }
        else cam.damping = 20f;

        if (!airplane.rightWingOperable && airplane.leftWingOperable && airplane.engineOperable)
        {
            rollAngle = Quaternion.AngleAxis((rollAmount + 90) * Time.deltaTime, Vector3.back);
            newRotation = Quaternion.Slerp(aircraft.rotation, totalAngle, turningRate * 50f);
            aircraft.rotation = currentRotation * newRotation;
            cam.damping = 5f;
        }
        else cam.damping = 20f;

        // Used for standard flight, else if engine fails
        if (lMB && airplane.allPartsOperable)
        {
            SetAirplaneAngle(mousePos.x, mousePos.y);
            newRotation = Quaternion.Slerp(aircraft.rotation, totalAngle, turningRate);
            aircraft.rotation = currentRotation * newRotation;
            cam.damping = 20f;
        }
        else if (!airplane.engineOperable)
        {
            aircraft.rotation = Quaternion.Slerp(aircraft.rotation, fallAngle, Time.deltaTime / 4f);
            cam.damping = 5f;
        }
    }

    void FindMousePosition()
    {
        // Find Mouse Position by moving the screen origin from bottom left to the center,
        // Then get the mouse position's distance coordinates away from the origin.
        
        /*
        originPos = Camera.main.ScreenToViewportPoint(new Vector3(
            (Camera.main.scaledPixelWidth / 2f),
            (Camera.main.scaledPixelHeight / 3f),
            Camera.main.nearClipPlane));
        */

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
}
