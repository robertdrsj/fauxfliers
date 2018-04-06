using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour {

    // Flag if a collision has been detected. Used mainly for HealthScript.
    // Attach this script to the child collider objects of the parent vehicle.

    AirplaneController airplane;

    private void Start()
    {
        airplane = FindObjectOfType<AirplaneController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        airplane.collisionDetected = true;
    }

    void OnCollisionExit(Collision collision)
    {
        airplane.collisionDetected = false;
    }
}
