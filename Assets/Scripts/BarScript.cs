using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{

    private float fillAmount;

    [SerializeField]
    private Image content;

    public float MaxValue { get; set; }

    public float Value
    {
        set
        {
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }

    void Update()
    {

        HandleBar();

    }

    private void HandleBar()
    {
        if (fillAmount != content.fillAmount)
            content.fillAmount = fillAmount;


    }

    // Function that translates health (0 to 100) to fillAmount (0 to 1).
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return ((value - inMin) * (outMax - outMin)) / (inMax - inMin) + outMin;
    }
}
