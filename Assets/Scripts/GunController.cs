using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    // Initialize //
    public BulletController bullet;                 // Can be used to change bullet types.
    public Transform firePoint;                     // Always make an empty child object of whatever has bullet output, and make the child object the bullet output. Drag that object here in inspector.

    // General Variables //
    public bool isFiring;                           // Flags when gun is being fired.
    public float bulletSpeed;                       // Speed of bullet.

    public float timeBetweenShots;                  // How much time should pass in between each bullet fired.
    private float shotCounter;                      // Keeps track of how much time passes in between bullets fired.
	
	void Update () {

        if (isFiring)
            FireGun();
        else
            shotCounter = 0f;
	}

    void FireGun()
    {
        shotCounter -= Time.deltaTime;

        if (shotCounter <= 0f)
        {
            shotCounter = timeBetweenShots;
            BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as BulletController;
            newBullet.bulletSpeed = bulletSpeed;
        }
    }
}
