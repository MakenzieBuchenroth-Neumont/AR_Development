using UnityEngine;

public class PatrolState : EnemyState
{
    private EnemyController _controller;
    private Vector3 targetPosition;
    private Vector3 patrolCenter;
    private EnemyData enemyData;
    private Transform playerPosition;
    private float playerDetectionRadius = 5.0f; // Hardcoded for now (should be moved into some sort of player controller)

    private bool isWaiting = false;
    private float waitTimer = 0f;
    public PatrolState(EnemyController controller, EnemyData data) : base(controller) 
    {
        this._controller = controller;
        this.enemyData = data;
        this.patrolCenter = controller.transform.position;

        if (EnemyManager.Instance)
        {
            playerPosition = EnemyManager.Instance.playerTransform;
        }
    }

    public override void OnEnter() 
    {
        _controller.ensnareProgress = 0f; // Reset ensnare progress when entering patrol state
        _controller.isEnsnared = false;
        //targetPosition = GetRandomPoint();
    }
    public override void OnExit() { }
    public override void UpdateState()
    {
        //if (targetPosition != null)
        //{
        //    if (isWaiting)
        //    {
        //        waitTimer -= Time.deltaTime;

        //        if (waitTimer <= 0f)
        //        {
        //            // Done waiting — get new point and resume movement
        //            targetPosition = GetRandomPoint();
        //            isWaiting = false;
        //        }
        //        return; // Skip movement while waiting
        //    }

        //    controller.movement.MoveTo(targetPosition);

        //    if (Vector3.Distance(controller.transform.position, targetPosition) < 0.5f)
        //    {
        //        // Reached target, start waiting
        //        isWaiting = true;
        //        waitTimer = enemyData.waitTime;
        //    }
        //}
    }

    /// <summary>
    /// Generates a random point within the patrol radius around the patrol center.
    /// Loops until a point is generated outside a certain radius around the player to avoid moving too close to the player
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = new Vector3();
        float tries = 0;
        do
        {
            tries++;
            float angle = Random.Range(0f, 360f);
            float x = patrolCenter.x + enemyData.patrolRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = patrolCenter.z + enemyData.patrolRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            //float y = patrolCenter.y + Random.Range(playerPosition.position.y, playerPosition.position.y * 1.5f) + 0.5f; // Random height based on player position to avoid spawning underground
            randomPoint = new Vector3(x, 0.15f, z);
            if (tries > 15)
            {
                Debug.LogWarning("GetRandomPoint: Too many attempts to find a valid point, returning last generated point.");
                break; // Avoid infinite loop if no valid point is found after many tries
            }
        } while (!IsPointInValidRange(randomPoint)); // Keep generating until a valid point is found

        return randomPoint;
    }

    private bool IsPointInValidRange(Vector3 point)
    {
        if (playerPosition == null) return false; // If player position is not set, assume point is valid

        float distance = Vector3.Distance(point, playerPosition.position);

        float minDistance = 2f;
        float maxDistance = _controller.enemyData.patrolRadius;

        return distance > minDistance && distance <= maxDistance;
    }

    /// <summary>
    /// Checks if a point is within a certain radius around the player.
    /// If it is, return true and generate new point
    /// If not, return false (accept the point as valid)
    /// </summary>
    /// <param name="point"></param>
    /// <returns>True if within radius, else False</returns>
    private bool IsPointInPlayerRadius(Vector3 point)
    {
        if (playerPosition == null) return false;
        float distance = Vector3.Distance(point, playerPosition.position);
        return distance <= playerDetectionRadius;
    }
}
