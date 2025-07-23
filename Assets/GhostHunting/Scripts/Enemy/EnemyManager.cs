using UnityEngine;

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
    [SerializeField]
    public EnemyDatabase enemyDb;

    public float spawnRadius = 100f;
    public Transform playerTransform;

    // Only for testing
    public void Start()
    {
        if (enemyDb == null)
        {
            Debug.LogError("EnemyDatabase is not assigned in the EnemyManager.");
            return;
        }
        SpawnRandomEnemies(10);
    }

    /* 1. Get random spawn location
     * 2. Generate random ghost data type (different prefab = equal spawn rate; Random num between 0 and enemyTypes.Length)
     * 3. Generate either base or void variant (base is common, void is rare)
     * 4. Instantiate either base or void prefab at the random location
     * 5. Add EnemyController and EnemyMovement components to the instantiated prefab
     * 6. Initialize the EnemyController with the selected enemy data
     * 7. Initialize the EnemyMovement with the movement speed and rotation speed from the selected enemy data
     */
    public void SpawnRandomEnemies(int numEnemies)
    {
        for (int i = 0; i < numEnemies; i++)
        {
            EnemyVariant variance = CalculateVariance();
            EnemyData enemyData = null;
            int random;
            switch (variance)
            {
                case EnemyVariant.Base:
                    random = Random.Range(0, enemyDb.baseEnemies.Length);
                    enemyData = enemyDb.baseEnemies[random];
                    break;
                case EnemyVariant.Void:
                    random = Random.Range(0, enemyDb.voidVarianceEnemies.Length);
                    enemyData = enemyDb.voidVarianceEnemies[random];
                    break;
            }

            // Instantiate the enemy at a random position around the player
            Vector3 spawnPoint = GenerateRandomSpawnPoint();

            GameObject enemyObj = Instantiate(enemyData.enemyPrefab, spawnPoint, Quaternion.identity);
            EnemyController controller = enemyObj.AddComponent<EnemyController>();
            EnemyMovement movement = enemyObj.AddComponent<EnemyMovement>();

            // Initialize the enemy controller and movement with the selected data
            controller.Initialize(enemyData);
            movement.Initialize(enemyData.movementSpeed, enemyData.rotationSpeed);

            Debug.Log($"Spawned {enemyData.name} at {spawnPoint} with variant {variance}");
        }
    }

    private Vector3 GenerateRandomSpawnPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * spawnRadius;
        randomPoint.y = 0; // Keep it on the ground
        randomPoint += playerTransform.position; // Offset from player position
        return randomPoint;
    }

    private EnemyVariant CalculateVariance()
    {
        int random = Random.Range(0, 101);
        // Common Type == 0 - 75
        // Rare Type == 76-100
        if (random >= 0 && random <= 75)
        {
            // common type
            return EnemyVariant.Base;
        }
        else if (random >= 76)
        {
            return EnemyVariant.Void;
        }
        else { return EnemyVariant.Base; }
    }
}
