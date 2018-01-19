using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeController : MonoBehaviour {

    // Public variables 
    public Transform smokePuff;
    public float timeBetweenPuffs;          // How much time should pass in between smoke puffs.

    // Private variables
    PlayerController player;
    float puffCounter;                      // Keeps track of how much time has passed since last smoke puff.

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void Update () {

        if (player.isThrusting)
            ThrustSmoke();

        if (player.durability.CurrentVal <= 25f)
            BrokenSmoke();

        if (player.isBroken)
            BrokenSmoke();

        if (Input.GetMouseButtonDown(1) && player.isBroken)
            ThrustSmoke();
        if (Input.GetMouseButton(1) && player.isBroken && player.durability.CurrentVal >= 85)
            ThrustSmoke();
    }

    void ThrustSmoke()
    {
        if (smokePuff.name == "ThrustSmoke")
        {
            puffCounter -= Time.deltaTime;

            if (puffCounter <= 0f)
            {
                puffCounter = timeBetweenPuffs;
                Instantiate(smokePuff, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            }
        }
    }

    void BrokenSmoke()
    {
        if (smokePuff.name == "BrokenSmoke")
        {
            puffCounter -= Time.deltaTime;

            if (puffCounter <= 0f)
            {
                puffCounter = Random.Range(timeBetweenPuffs - 0.1f, timeBetweenPuffs + 0.1f);
                Instantiate(smokePuff, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            }
        }
    }
}
