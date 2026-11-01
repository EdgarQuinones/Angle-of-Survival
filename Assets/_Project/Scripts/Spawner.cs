using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject enemyPrefab;
    public GameObject player;

    [Header("Timing Settings")]
    public float totalGameTime = 30f; // How long the spawner runs
    public float startSpawnInterval = 5.0f; // Seconds between spawns at the START
    public float minSpawnInterval = 2.0f;   // The fastest it can ever get (Cap)
    public float speedUpAmount = 0.1f;      // How much faster to get every spawn

    private void Start()
    {
        // Start the smart loop
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float currentInterval = startSpawnInterval;
        float startTime = Time.time;

        // Loop until the game time is over
        while (Time.time < startTime + totalGameTime)
        {
            SpawnEnemy();

            // Wait for the current interval
            yield return new WaitForSeconds(currentInterval);

            // MATH: Make the next interval smaller (faster), but don't go below the minimum
            currentInterval -= speedUpAmount;
            if (currentInterval < minSpawnInterval)
            {
                currentInterval = minSpawnInterval;
            }
        }

        Debug.Log("30 Seconds up! Spawning stopped.");
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // Assign player to the new enemy
        FollowPlayer enemyScript = newEnemy.GetComponent<FollowPlayer>();
        if (enemyScript != null)
        {
            enemyScript.player = this.player;
        }
    }
}