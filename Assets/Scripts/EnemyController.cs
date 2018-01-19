using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    // Public variables
    // Animation //
    public Animator anim;
    public bool isHurt;

    // Gun Stuff //

    public float enemyThrust;
    public float enemyGravity;

    public AudioSource bulletCollideSound;
    public AudioSource playerCollideSound;
    public int startingPitch = 4;
    public int timeToDecrease = 5;

    // General Variables //
    public float health;
    public float contactDamage;
    public float explosionDamage;

    // Private Variables
    public GameObject[] BorderParent;
    Rigidbody2D enemy;
    BulletController bullet;
    PlayerController player;
    ExplosionController explosion;
    ScreenshakeController screenshake;
    AltimeterController altimeter;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        explosion = GetComponent<ExplosionController>();
        screenshake = FindObjectOfType<ScreenshakeController>();
        altimeter = FindObjectOfType<AltimeterController>();
        anim = GetComponent<Animator>();
        enemy = GetComponent<Rigidbody2D>();
        bulletCollideSound = GetComponentInChildren<AudioSource>();
        playerCollideSound = GetComponentInChildren<AudioSource>();

        enemy.AddForce(-1 * transform.up * enemyThrust * (altimeter.timeCurrent + 5));
    }

    void Update()
    {
        anim.SetBool("isHurt", isHurt);
        if (isHurt)
            isHurt = false;

        if (health <= 0)
        {
            Explode();
        }

        // Increase enemy move speed as stage progresses.
        if (altimeter.sliderAmt >= .20f)
            enemy.gravityScale = enemyGravity + .10f;
        if (altimeter.sliderAmt >= .40f)
            enemy.gravityScale = enemyGravity + .20f;
        if (altimeter.sliderAmt >= .60f)
            enemy.gravityScale = enemyGravity + .30f;
        if (altimeter.sliderAmt >= .80f)
            enemy.gravityScale = enemyGravity + .40f;
    }

    void OnCollisionEnter2D(Collision2D impact)
    {
        if (impact.gameObject.tag == "Friendly")
        {
            // Animate
            isHurt = true;

            // Other
            bulletCollideSound.Play();
            Destroy(impact.gameObject);
            bullet = impact.collider.GetComponent<BulletController>();
            health -= bullet.fBulletDamage;
        }

        if (impact.gameObject.tag == "Player")
        {
            // Animate
            isHurt = true;

            // Other
            playerCollideSound.Play();
            player = impact.collider.GetComponent<PlayerController>();
            player.health.CurrentVal -= contactDamage;
            this.health -= 2;
            screenshake.shakeAmount = 0.25f;
            screenshake.shakeDuration = 0.1f;
        }

        if (impact.gameObject.tag == "Wall")
        {
            Physics2D.IgnoreCollision(impact.collider, GetComponent<Collider2D>());
        }
    }

    void Explode()
    {
        Destroy(gameObject);
        explosion.Animate();
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}