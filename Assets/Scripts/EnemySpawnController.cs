using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour {

    [SerializeField]
    public GameObject[] enemies;
    public Vector3 spawnValues;
    public Transform EnemyParent;

    AltimeterController altimeter;

    public float levelTimeRemaining = 60f;
    public float spawnRate;
    public float timeLeftToSpawn;
    public float spawnWait;
    public float enemy1Chance;
    public float enemy2Chance;
    public float enemy3Chance;
    public float enemy4Chance;
    public float enemy5Chance;
    private float totalEnemySpawnChance;

    enum state { ENABLED, RUNNING, DISABLED };
    private state spawnState;


    void Start()
    {
        altimeter = FindObjectOfType<AltimeterController>();

        timeLeftToSpawn = levelTimeRemaining - 10f;
        totalEnemySpawnChance = enemy1Chance + enemy2Chance + enemy3Chance + enemy4Chance + enemy5Chance;
        StartCoroutine(enemySpawn());
    }

    void Update()
    {
        timeLeftToSpawn -= Time.deltaTime;

        if (altimeter.sliderAmt >= .20f)
            spawnRate = .15f;
        if (altimeter.sliderAmt >= .40f)
            spawnRate = .12f;
        if (altimeter.sliderAmt >= .60f)
            spawnRate = .09f;
        if (altimeter.sliderAmt >= .80f)
            spawnRate = .06f;
    }

    IEnumerator enemySpawn()
    {
        while (timeLeftToSpawn > 0)
        {
            // Random amount of enemies bounded by [1, 3]
            int enemyCount = Random.Range(1, 3);
            for (int j = 0; j < enemyCount; j++)
                SpawnEnemy();
            yield return new WaitForSeconds(spawnRate);
        }

        yield break;
    }

    void SpawnEnemy()
    {
        // Choose random enemy to spawn
        float enemyChoice = Random.Range(1f, totalEnemySpawnChance);

        spawnValues = new Vector3(Random.Range(-16, 16), 18, 0f);

        if (enemyChoice <= enemy1Chance)
        {
            Instantiate(enemies[0], spawnValues, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
        else if (enemyChoice <= (enemy1Chance + enemy2Chance) && enemyChoice > enemy1Chance)
        {
            Instantiate(enemies[1], spawnValues, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
        else if (enemyChoice <= (enemy1Chance + enemy2Chance + enemy3Chance) && enemyChoice > (enemy1Chance + enemy2Chance))
        {
            Instantiate(enemies[2], spawnValues, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
        else if (enemyChoice <= (enemy1Chance + enemy2Chance + enemy3Chance + enemy4Chance) && enemyChoice > (enemy1Chance + enemy2Chance + enemy3Chance))
        {
            Instantiate(enemies[3], spawnValues, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
        else if (enemyChoice > (enemy1Chance + enemy2Chance + enemy3Chance))
        {
            Instantiate(enemies[4], spawnValues, Quaternion.identity);
        }
    }
}
