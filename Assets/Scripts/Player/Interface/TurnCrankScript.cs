using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCrankScript : MonoBehaviour {

    // Initialize
    PlayerController player;
    AirplaneController airplane;

    // Setup
    public bool isLeftCrank;
    bool leftCrankInteractable;
    public bool isRightCrank;
    public bool rightCrankInteractable;

    [SerializeField]
    Vector3 crankPos;
    [SerializeField]
    Vector3 mousePos;
    [SerializeField]
    float crankToMouseDis;
    public float distanceLimit;
    float crankToMouseRot;

    // Crank rotation for SetGoal().
    Quaternion lastRot;
    Quaternion currentRot;
    public float rotationDiff;
    bool cW;                             // Clockwise
    bool cCW;                            // Counter-CW

    public float minRandomRotation;
    public float maxRandomRotation;
    public float rotationGoalAmount;
    public float rotationGoalCurrent;

    // AutoRotate()
    public bool rotateZ;
    public float zDegreesPerSecond;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
    {
        player = FindObjectOfType<PlayerController>();
        airplane = FindObjectOfType<AirplaneController>();

        SetRandomGoal();

        leftCrankInteractable = true;
        rightCrankInteractable = true;
	}
	
	void FixedUpdate()
    {
        //LeftCheck();
        //RightCheck();
        FindMousePosition();

        // Left Crank
        if (isLeftCrank && leftCrankInteractable)
        {
            lastRot = transform.localRotation;                                      // Used for ClockwiseCheck().

            if (player.lMB && player.doNotInput)
                ManageLeftRotation();
            else
            {
                rotateZ = true;
                AutoRotate(zDegreesPerSecond);
            }

            currentRot = transform.localRotation;                                   // Used for ClockwiseCheck().
            //ClockwiseCheck();
            rotationDiff = Mathf.Abs(currentRot.z) - Mathf.Abs(lastRot.z);          // Used for DeductFromGoal().
        }

        // Right Crank
        if (isRightCrank && rightCrankInteractable)
        {
            lastRot = transform.localRotation;                                      // Used for ClockwiseCheck().

            if (player.lMB && player.doNotInput)
                ManageRightRotation();
            else
            {
                rotateZ = true;
                AutoRotate(zDegreesPerSecond);
            }

            currentRot = transform.localRotation;                                   // Used for ClockwiseCheck().
            //ClockwiseCheck();
            rotationDiff = Mathf.Abs(currentRot.z) - Mathf.Abs(lastRot.z);          // Used for DeductFromGoal().
        }
    }

    // Checks if wing is broken and can be interacted with.
    void LeftCheck()
    {
        if (isLeftCrank)
        {
            if (!airplane.leftWingOperable)
            {
                leftCrankInteractable = true;
            }
            else
            {
                leftCrankInteractable = false;
                transform.Rotate(new Vector3(0f, 0f, 0f));
            }
        }
    }

    void RightCheck()
    {
        if (isRightCrank)
        {
            if (!airplane.rightWingOperable)
            {
                rightCrankInteractable = true;
            }
            else
            {
                rightCrankInteractable = false;
                transform.Rotate(new Vector3(0f, 0f, 0f));
            }
        }
    }
    
    // Randomly sets the degree amount for the player to rotate the crank before it's fixed.
    void SetRandomGoal()
    {
        float randomRotation = Random.Range(minRandomRotation, maxRandomRotation);
        rotationGoalAmount = randomRotation;
        rotationGoalCurrent = rotationGoalAmount;
    }

    // Reduces the amount of turning necessary to fully repair the airplane part.
    void DeductFromLeftGoal()
    {
        // Deducts from the current rotation goal, whether its CW or CCW (bc even if you go CCW, the degrees are messed up so it'll think you're CW sometimes).
        if (Mathf.Sign(rotationDiff) == 1)
            rotationGoalCurrent -= rotationDiff;
        if (Mathf.Sign(rotationDiff) == -1)
            rotationGoalCurrent -= -rotationDiff;

        // Reset deduction so that when the player is no longer turning the crank, the current rotation goal won't continue to be deducted.
        rotationDiff = 0f;

        if (rotationGoalCurrent <= 0f)
        {
            leftCrankInteractable = false;
            SetRandomGoal();
        }
    }

    void DeductFromRightGoal()
    {
        // Deducts from the current rotation goal, whether its CW or CCW (bc even if you go CCW, the degrees are messed up so it'll think you're CW sometimes).
        if (Mathf.Sign(rotationDiff) == 1)
            rotationGoalCurrent -= rotationDiff;
        if (Mathf.Sign(rotationDiff) == -1)
            rotationGoalCurrent -= -rotationDiff;

        // Reset deduction so that when the player is no longer turning the crank, the current rotation goal won't continue to be deducted.
        rotationDiff = 0f;

        if (rotationGoalCurrent <= 0f)
        {
            rightCrankInteractable = false;
            SetRandomGoal();
        }
    }

    // Finds the mouse cursor position based on the crank's origin position.
    void FindMousePosition()
    {
        crankPos = Camera.main.WorldToViewportPoint(transform.position);
        mousePos = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        // The viewport's X is stretched a little further which skews the radius, so it's been buffed up a bit.
        crankToMouseDis = Mathf.Sqrt((Mathf.Pow((crankPos.x - mousePos.x), 2) * 3f) + Mathf.Pow((crankPos.y - mousePos.y), 2)) * 100f;
        if (isLeftCrank)
            crankToMouseRot = (Mathf.Atan2(crankPos.y - mousePos.y, crankPos.x - mousePos.x) * Mathf.Rad2Deg) + 1200f;
        if (isRightCrank)
            crankToMouseRot = (Mathf.Atan2(crankPos.y - mousePos.y, crankPos.x - mousePos.x) * Mathf.Rad2Deg) + 60f;
    }

    // ** Does not work right. Likely due to the z-rotation switching from positive to negative (and vice versa) at a random spot.
    void ClockwiseCheck()
    {
        if (leftCrankInteractable && rotateZ)
        {
            if (Mathf.Sign(Mathf.Sin(Mathf.Abs(currentRot.z) - Mathf.Abs(lastRot.z))) == 1)
            {
                cCW = true;
                cW = false;
            }
        }
        else
            cCW = false;

        if (rightCrankInteractable && rotateZ)
        {
            if (Mathf.Sign(Mathf.Sin(Mathf.Abs(currentRot.z) - Mathf.Abs(lastRot.z))) == -1)
            {
                cW = true;
                cCW = false;
            }
        }
        else
            cW = false;
    }

    // Keeps track of how far the player has turned the crank.
    void ManageLeftRotation()
    {
        if (crankToMouseDis <= distanceLimit)
        {
            rotateZ = false;
            transform.localRotation = Quaternion.Euler(0f, 0f, crankToMouseRot);
            DeductFromLeftGoal();
        }
        else
        {
            rotateZ = true;
            AutoRotate(zDegreesPerSecond);
        }
    }

    void ManageRightRotation()
    {
        if (crankToMouseDis <= distanceLimit)
        {
            rotateZ = false;
            transform.localRotation = Quaternion.Euler(0f, 0f, -crankToMouseRot);
            DeductFromRightGoal();
        }
        else
        {
            rotateZ = true;
            AutoRotate(zDegreesPerSecond);
        }
    }

    // Automatically rotates the crank when the wing is broken but not being interacted with.
    void AutoRotate(float rotationRate)
    {
        transform.Rotate(Vector3.forward, rotationRate * Time.deltaTime, Space.Self);
    }
}
