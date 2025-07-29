using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory _instance;
    public static bool IsInitialized => _instance != null;
    private PlayerInventory() { }

    public List<GameObject> capturedGhosts = new List<GameObject>();
    public List<GameObject> encounteredGhosts = new List<GameObject>();

    private List<GameObject> allCapturedBaseGhosts = new List<GameObject>();
    private List<GameObject> allCapturedVoidGhosts = new List<GameObject>();

    private List<GameObject> allEncounteredBaseGhosts = new List<GameObject>();
    private List<GameObject> allEncounteredVoidGhosts = new List<GameObject>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEncounteredGhost(GameObject ghost)
    {
        if (ghost != null && !encounteredGhosts.Contains(ghost))
        {
            encounteredGhosts.Add(ghost);
            switch (ghost.GetComponent<EnemyController>().enemyData.variant)
            {
                case EnemyVariant.Base:
                    if (!allEncounteredBaseGhosts.Contains(ghost))
                        allEncounteredBaseGhosts.Add(ghost);
                    break;
                case EnemyVariant.Void:
                    if (!allEncounteredVoidGhosts.Contains(ghost))
                        allEncounteredVoidGhosts.Add(ghost);
                    break;
            }
        }
    }
    public void AddGhostToInventory(GameObject ghost)
    {
        if (ghost != null && !capturedGhosts.Contains(ghost))
        {
            capturedGhosts.Add(ghost);
            switch (ghost.GetComponent<EnemyController>().enemyData.variant)
            {
                case EnemyVariant.Base:
                    if (!allCapturedBaseGhosts.Contains(ghost))
                        allCapturedBaseGhosts.Add(ghost);
                    break;
                case EnemyVariant.Void:
                    if (!allCapturedVoidGhosts.Contains(ghost))
                        allCapturedVoidGhosts.Add(ghost);
                    break;
            }

            ghost.SetActive(false);
        }
    }
    public int GetCapturedAmountByType(EnemyType type, EnemyVariant variant)
    {
        if (!IsInitialized)
        {
            Debug.LogError("PlayerInventory is not initialized. Cannot get amount by type.");
            return 0;
        }

        int count = 0;
        foreach (GameObject ghost in capturedGhosts)
        {
            EnemyController controller = ghost.GetComponent<EnemyController>();
            if (controller != null && controller.enemyData.type == type && controller.enemyData.variant == variant)
            {
                count += 1; 
            }
        }
        return count;
    }
    public int GetEncounteredAmountByType(EnemyType type, EnemyVariant variant)
    {
        if (!IsInitialized)
        {
            Debug.LogError("PlayerInventory is not initialized. Cannot get amount by type.");
            return 0;
        }

        int count = 0;
        foreach (GameObject ghost in encounteredGhosts)
        {
            EnemyController controller = ghost.GetComponent<EnemyController>();
            if (controller != null && controller.enemyData.type == type && controller.enemyData.variant == variant)
            {
                count += 1; 
            }
        }
        return count;
    }
}
