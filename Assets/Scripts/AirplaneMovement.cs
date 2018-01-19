using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneMovement : MonoBehaviour {

    public GameObject leftThruster;
    public GameObject rightThruster;
    public GameObject mainThruster;

    public GameObject leftWing;
    public GameObject rightWing;
    public GameObject mainTrim;
    public GameObject primaryHull;

    public float standardThrustForce;
    float leftThrustForce;
    float rightThrustForce;
    float mainThrustForce;

	void Start () {

        leftThruster    = GetComponent<GameObject>();
        rightThruster   = GetComponent<GameObject>();
        mainThruster    = GetComponent<GameObject>();
        leftWing        = GetComponent<GameObject>();
        rightWing       = GetComponent<GameObject>();
        mainTrim        = GetComponent<GameObject>();
        primaryHull     = GetComponent<GameObject>();

    }
	
	void FixedUpdate () {
		
	}



}
