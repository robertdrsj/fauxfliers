using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    // Initialize
    PlayerController player;
    ScreenshakeController screenshake;

    // General Variables
    public float bulletSpeed;

    public float eBulletDamage;                     // Enemy bullet damage vs player.
    public float fBulletDamage;                     // Friendly bullet damage vs enemies.


    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        screenshake = FindObjectOfType<ScreenshakeController>();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.gameObject.GetComponent<Collider2D>());
        screenshake.shakeAmount = 0.08f;
        screenshake.shakeDuration = 0.08f;
    }

    void Update ()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
