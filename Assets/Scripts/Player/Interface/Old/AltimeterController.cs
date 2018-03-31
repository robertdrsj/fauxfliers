using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltimeterController : MonoBehaviour {

    [SerializeField]
    Slider altitude;
    public float sliderAmt;

    //public bool timerOn;

    public float timeCurrent;
    public float timeMax;

    private void Start()
    {
        altitude = FindObjectOfType<Slider>();
    }

    void Update()
    {
        timeCurrent += Time.deltaTime;

        sliderAmt = Map(timeCurrent, 0, timeMax, 0, 1);

        altitude.value = sliderAmt;

        if (timeCurrent >= timeMax)
        {
            // You win the stage.
            Debug.Log("Stage Success!!");

            // Ignore below.
            //Application.LoadLevel("gameOver");
        }
    }

    // Function that translates health (0 to 100) to slider value (0 to 1).
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return ((value - inMin) * (outMax - outMin)) / (inMax - inMin) + outMin;
    }
}
