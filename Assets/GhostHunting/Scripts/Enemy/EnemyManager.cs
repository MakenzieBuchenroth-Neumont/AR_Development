using UnityEngine;
using UnityEngine.UI;
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
    public static EnemyManager Instance { get; private set; }
    public static bool IsInitialized => Instance != null;

    [Header("Enemy Settings")]
    [SerializeField] private EnemyDatabase enemyDb;
    [SerializeField] private int minGhosts = 3;
    [SerializeField] private int maxGhosts = 10;
    [SerializeField] private List<EnemyData> enemyList;
    public GameObject spawnedEnemy;
    public EnemyController spawnedEnemyController;

    [Header("Spawn Settings")]
    [SerializeField] private float resetTimeMinutes = 2;
    [SerializeField] private float spawnRadius = 100f;
    private float timeRemaining = 0f;
    private bool timerRunning = false;
    public bool canScan = true;
    public Transform playerTransform;

    [Header("UI Settings")]
    [SerializeField] private SwipeTracker swipeTracker;
    [SerializeField] private Button scanButton;

    private EnemyManager() { }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Keep the manager persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
    public void Start()
    {
        if (enemyDb == null)
        {
            Debug.LogError("EnemyDatabase is not assigned in the EnemyManager.");
            return;
        }

        Camera arCamera = Camera.main;
        if (arCamera)
        {
            playerTransform = arCamera.transform;
        }
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //if (player != null)
        //{
        //    playerTransform = player.transform;
        //}
        //else
        //{
        //    Debug.LogError("Player GameObject not found in the scene. EnemyManager requires a player reference.");
        //}

        GenerateEnemyList(Random.Range(minGhosts, maxGhosts + 1));
        ResetTimer();
        canScan = true;
    }
    public void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timerRunning = false;
                // RemoveEnemies(); // Remove all enemies from the scene whent the timer runs out
                GenerateEnemyList(Random.Range(minGhosts, maxGhosts + 1));
                ResetTimer();
            }
        }

        if (spawnedEnemy != null)
        {
            CheckIfEnsnared();
            CheckIfCaptured();
        }
    }

    /// <summary>
    /// Spawns an enemy at a random position around the player from the enemy list
    /// Removes the enemy data from the list to prevent respawning the same enemy
    /// Gives the enemy a unique ID based on the current count of the spawned enemies
    /// Adds the enemy to the spawned enemies list for tracking and management
    /// </summary>
    private void SpawnEnemy()
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

        // Handle UI
        spawnedEnemy = enemyObj;
        if (spawnedEnemy.TryGetComponent(out EnemyController enemyController))
        {
            spawnedEnemyController = enemyController;
        }

        if (PlayerInventory.IsInitialized)
        {
            PlayerInventory._instance.AddEncounteredGhost(spawnedEnemy);
        }

        if (spawnedEnemy) canScan = false;
        ToggleButton(scanButton, false);

        if (SwipeTracker.IsInitialized)
        {
            SwipeTracker._instance.canEnsnare = true;
        }
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
        //GameObject spawnedEnemy = null;
        //if (spawnedEnemies != null)
        //{
        //    foreach (GameObject enemy in spawnedEnemies)
        //    {
        //        EnemyController controller = enemy.GetComponent<EnemyController>();
        //        if (controller != null && controller.id == id)
        //        {
        //            spawnedEnemy = enemy;
        //            break;
        //        }
        //    }
        //}
        //return spawnedEnemy;
        return null;
    }

    /// <summary>
    /// Removes a spawned enemy GameObject by its unique ID from the SpawnedEnemy list.
    /// </summary>
    /// <param name="id">Identifier of the game object</param>
    /// <returns></returns>
    private GameObject RemoveSpawnedEnemyById(int id)
    {
        //GameObject enemyToRemove = GetSpawnedEnemyById(id);
        //if (enemyToRemove != null)
        //{
        //    spawnedEnemies.Remove(enemyToRemove);
        //    Destroy(enemyToRemove);
        //    return enemyToRemove;
        //}
        //else
        //{
        //    Debug.LogWarning("No enemy found with ID: " + id);
        //    return null;
        //}
        return null;
    }

    /// <summary>
    /// Removes all spawned enemies from the scene and clears the spawnedEnemies list.
    /// </summary>
    private void RemoveEnemies()
    {
        // If there is an enemy currently spawned, don't remove the enemy
        //if (spawnedEnemy == null)
        //{

        //}

        //if (spawnedEnemies != null)
        //{
        //    foreach (GameObject enemy in spawnedEnemies)
        //    {
        //        if (enemy != null)
        //        {
        //            Destroy(enemy);
        //        }
        //    }
        //    spawnedEnemies.Clear();
        //}
        //else
        //{
        //    Debug.LogWarning("No enemies to remove.");
        //}
    }
    #endregion

    #region Spawn, Ensnare & Capture Management
    public void AttemptScan()
    {
        if (enemyList == null || enemyList.Count == 0)
        {
            // No enemies available to spawn
            Debug.LogWarning($"No enemies available to spawn. Try again in {timeRemaining * 60f} minutes");
            // Handle more logic later (e.g., UI message and handling)
        }

        if (canScan && spawnedEnemy == null)
        {
            SpawnEnemy();
        }
    }
    private Vector3 GenerateRandomSpawnPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * spawnRadius;
        randomPoint.y = 0; // Keep it on the ground
        randomPoint += playerTransform.position; // Offset from player position
        return new Vector3(randomPoint.x, 0.15f, randomPoint.z);
    }
    private void ResetTimer()
    {
        timeRemaining = resetTimeMinutes * 60f;
        timerRunning = true;
    }
    public void EnemyEscaped()
    {
        if (SwipeTracker.IsInitialized)
        {
            SwipeTracker._instance.canEnsnare = true;
            SwipeTracker._instance.canThrowTrap = false;
        }
    }
    public void EnemyCaptured()
    {
        if (spawnedEnemy == null)
        {
            canScan = true;
            ToggleButton(scanButton, true);
        }
        else
        {
            spawnedEnemy = null;
            canScan = true;
            ToggleButton(scanButton, true);
        }

        if (SwipeTracker.IsInitialized)
        {
            SwipeTracker._instance.ResetState();
        }
    }

    #endregion

    #region UI Management
    private void ToggleButton(Button button, bool enable)
    {
        if (button != null)
        {
            button.interactable = enable;
        }
    }
    private void CheckIfEnsnared()
    {
        EnemyController controller = spawnedEnemy.GetComponent<EnemyController>();
        if (controller != null && controller.isEnsnared)
        {
            swipeTracker.canEnsnare = false;
            swipeTracker.ghostToCapture = spawnedEnemy;
        }
    }
    private void CheckIfCaptured()
    {
        EnemyController controller = spawnedEnemy.GetComponent<EnemyController>();
        if (controller != null && controller.isCaptured)
        {
            // Handle later
        }
    }
    #endregion
}
