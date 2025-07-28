using UnityEngine;
using System.Collections.Generic;

// Will be assigned later
public enum EnemyType 
{
    Boobert,    // Classic White Sheet Ghost
    Witch,      // White Sheet with Witch Hat
    Mildew,     // Dirtied White Sheet Ghost
    Sulfur,     // Dirtied Sheet with Devil Horns
    Mustardo,   // Yellow Dirtied Sheet with Devil Horns
    Wanda,      // Yellow Dirtied Sheet with Witch Hat
    Axel        // White Sheet with Axe in Head and Blood
}

public enum EnemyVariant
{
    Base,   // Common
    Void    // Rare
}

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField]
    public EnemyDatabase enemyDb;
    public int minGhosts = 3;
    public int maxGhosts = 10;
    private List<EnemyData> enemyList;
    private List<GameObject> spawnedEnemies;
    public GameObject spawnedEnemy;

    [Header("Spawn Settings")]
    public float resetTimeMinutes = 2;
    public float spawnRadius = 100f;
    private float timeRemaining = 0f;
    private bool timerRunning = false;
    private Transform playerTransform;

    // Only for testing
    public void Start()
    {
        if (enemyDb == null)
        {
            Debug.LogError("EnemyDatabase is not assigned in the EnemyManager.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found in the scene. EnemyManager requires a player reference.");
        }

        GenerateEnemyList(Random.Range(minGhosts, maxGhosts + 1));
        ResetTimer();
        SpawnEnemy();
    }
    public void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0f)
            {
                // Debugging only: spawn enemy every 15 seconds
                //if (Mathf.Abs(timeRemaining % 15f) < 0.01f && Mathf.Abs(timeRemaining - 60f) > 0.01f)
                //{
                //    SpawnEnemy();
                //}
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timerRunning = false;
                RemoveAllEnemies(); // Remove all enemies from the scene whent the timer runs out
                GenerateEnemyList(Random.Range(minGhosts, maxGhosts + 1));
                ResetTimer();
            }
        }
    }

    /// <summary>
    /// Spawns an enemy at a random position around the player from the enemy list
    /// Removes the enemy data from the list to prevent respawning the same enemy
    /// Gives the enemy a unique ID based on the current count of the spawned enemies
    /// Adds the enemy to the spawned enemies list for tracking and management
    /// </summary>
    public void SpawnEnemy()
    {
        if (spawnedEnemy != null) return; // Only one enemy should be spawned at a time
        // Check if there are any enemies to spawn
        if (enemyList == null || enemyList.Count == 0)
        {
            Debug.LogWarning("No enemies available to spawn.");
            return;
        }

        // Get EnemyData From List
        EnemyData enemyData = enemyList[Random.Range(0, enemyList.Count)];
        enemyList.Remove(enemyData); // Remove the enemy from the list to prevent respawning the same one

        // Instantiate the enemy at a random position around the player
        Vector3 spawnPoint = GenerateRandomSpawnPoint();

        GameObject enemyObj = Instantiate(enemyData.enemyPrefab, spawnPoint, Quaternion.identity);
        EnemyController controller = enemyObj.AddComponent<EnemyController>();
        EnemyMovement movement = enemyObj.AddComponent<EnemyMovement>();

        // Initialize the enemy controller and movement with the selected data
        controller.Initialize(enemyData);
        movement.Initialize(enemyData.movementSpeed, enemyData.rotationSpeed);

        // Give the enemy a unique ID
        controller.id = spawnedEnemies != null ? spawnedEnemies.Count : 0; // Unique ID based on current count

        // Add the enemy to the spawned enemies list
        //if (spawnedEnemies == null)
        //{
        //    spawnedEnemies = new List<GameObject>();
        //}
        //spawnedEnemies.Add(enemyObj);
        spawnedEnemy = enemyObj;
    }

    #region Enemy Helpers
    /// <summary>
    /// Generates a list of enemy data based on the specified number of desired enemies.
    /// Generates a random variant and random type then stores it in the enemyList
    /// </summary>
    /// <param name="numEnemies"></param>
    private void GenerateEnemyList(int numEnemies)
    {
        enemyList = new List<EnemyData>();

        for (int i = 0; i < numEnemies; i++)
        {
            EnemyVariant variant = CalculateVariance();
            int randomIndex;
            switch (variant)
            {
                case EnemyVariant.Base:
                    randomIndex = Random.Range(0, enemyDb.baseEnemies.Length);
                    enemyList.Add(enemyDb.baseEnemies[randomIndex]);
                    break;
                case EnemyVariant.Void:
                    randomIndex = Random.Range(0, enemyDb.voidVarianceEnemies.Length);
                    enemyList.Add(enemyDb.voidVarianceEnemies[randomIndex]);
                    break;
            }
        }
    }
    /// <summary>
    /// Calculates the variance of the enemy type based on a random number.
    /// 1 - 85 = Common Type (Base)
    /// 86 - 100 = Rare Type (Void)
    /// </summary>
    /// <returns>Returns EnemyVariant</returns>
    private EnemyVariant CalculateVariance()
    {
        int random = Random.Range(0, 101);
        // Common Type == 0 - 75
        // Rare Type == 76-100
        if (random >= 0 && random <= 85)
        {
            // common type
            return EnemyVariant.Base;
        }
        else if (random > 85)
        {
            return EnemyVariant.Void;
        }
        else { return EnemyVariant.Base; }
    }

    /// <summary>
    /// Retrieves a spawned enemy GameObject by its unique ID.
    /// </summary>
    /// <param name="id">Identifier of the game object</param>
    /// <returns>Returns the game object found with the identifier or null</returns>
    private GameObject GetSpawnedEnemyById(int id)
    {
        GameObject spawnedEnemy = null;
        if (spawnedEnemies != null)
        {
            foreach (GameObject enemy in spawnedEnemies)
            {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller != null && controller.id == id)
                {
                    spawnedEnemy = enemy;
                    break;
                }
            }
        }
        return spawnedEnemy;
    }

    /// <summary>
    /// Removes a spawned enemy GameObject by its unique ID from the SpawnedEnemy list.
    /// </summary>
    /// <param name="id">Identifier of the game object</param>
    /// <returns></returns>
    private GameObject RemoveSpawnedEnemyById(int id)
    {
        GameObject enemyToRemove = GetSpawnedEnemyById(id);
        if (enemyToRemove != null)
        {
            spawnedEnemies.Remove(enemyToRemove);
            Destroy(enemyToRemove);
            return enemyToRemove;
        }
        else
        {
            Debug.LogWarning("No enemy found with ID: " + id);
            return null;
        }
    }

    /// <summary>
    /// Removes all spawned enemies from the scene and clears the spawnedEnemies list.
    /// </summary>
    private void RemoveAllEnemies()
    {
        if (spawnedEnemies != null)
        {
            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
            spawnedEnemies.Clear();
        }
        else
        {
            Debug.LogWarning("No enemies to remove.");
        }
    }
    #endregion
    private Vector3 GenerateRandomSpawnPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * spawnRadius;
        randomPoint.y = 0; // Keep it on the ground
        randomPoint += playerTransform.position; // Offset from player position
        return randomPoint;
    }
    private void ResetTimer()
    {
        timeRemaining = resetTimeMinutes * 60f;
        timerRunning = true;
    }
}
