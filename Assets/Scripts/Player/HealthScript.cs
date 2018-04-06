using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour {

    AirplaneController airplane;

    float minHealth = 0f;
    public float maxHealth;             // Health indicates overall airplane status. If HP is at 0, the whole airplane is destroyed.
    public float currentHealth;

    public float damageAmount;
    public float healAmount;

    [HideInInspector]
    public float gaugeRotation;

    void Start () {
        airplane = GetComponent<AirplaneController>();
	}
	
	void FixedUpdate () {
		if (airplane.enableHealth)
        {
            ManageHealth();
            GaugeCheck();
        }
	}

    void ManageHealth()
    {
        // If child colliders detect a collision, take damage.
        if (airplane.collisionDetected)
        {
            TakeDamageFor(damageAmount);
        }
    }

    public void HealFor(float healthRestored)
    {
        if (currentHealth < maxHealth)
            currentHealth += healthRestored;
        else
            currentHealth = maxHealth;
    }

    void TakeDamageFor(float damageTaken)
    {
        if (currentHealth > minHealth)
            currentHealth -= damageTaken;
        else
        {
            DestroyVehicle();
            GameOver();
        }
    }

    // Destroys the player's vehicle.
    void DestroyVehicle()
    {
        //Destroy(this);
    }

    // Causes the GameOver state from GameController. ***remove this later***
    void GameOver()
    {

    }

    // Provides the engine gauge with a rotation value between 0 and 1.
    void GaugeCheck()
    {
        gaugeRotation = currentHealth / maxHealth;
    }
}
