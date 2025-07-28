using TMPro;
using UnityEngine;

public class EnsnaringState : EnemyState
{
    private EnemyController enemyController;
    private Transform playerTransform;

    private Vector3 targetPosition;
    private Vector3 patrolCenter;
    private EnemyData enemyData;
    private Transform playerPosition;
    private float playerDetectionRadius = 5.0f; // Hardcoded for now (should be moved into some sort of player controller)

    private bool isWaiting = false;
    private float waitTimer = 0f;
    public EnsnaringState(EnemyController controller) : base(controller)
    {
        enemyController = controller;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found in the scene.");
        }
    }

    public override void OnEnter()
    {
        // Initialize ensnaring progress if needed
        targetPosition = GetRandomPoint();
    }
    public override void UpdateState()
    {
        // Check if the ensnaring progress is complete
        // If the enemy is ensnared, change to the ensnared state
        if (CheckEnsnareComplete())
        {
            enemyController.isEnsnared = true;
            enemyController.ChangeState(new EnsnaredState(enemyController));
            return; // Exit the update method
        }

        // Check how far away from the player the ghost is
        // If the ghost is far away from the player, move back to patrol state
        // Else, flee from the player
        //FleeFromPlayer();
        if (targetPosition != null)
        {
            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    // Done waiting — get new point and resume movement
                    targetPosition = GetRandomPoint();
                    isWaiting = false;
                }
                return; // Skip movement while waiting
            }

            controller.movement.MoveTo(targetPosition);

            if (Vector3.Distance(controller.transform.position, targetPosition) < 0.5f)
            {
                // Reached target, start waiting
                isWaiting = true;
                waitTimer = enemyData.waitTime;
            }
        }
    }

    public override void OnExit()
    {
        //
    }

    /// <summary>
    /// Flee from the player if within flee distance.
    /// If the player is too far away, return to patrol state without resetting ensnare progress.
    /// </summary>
    private void FleeFromPlayer()
    {
        Vector3 distanceToPlayer = controller.transform.position - playerTransform.position;
        if (distanceToPlayer.magnitude < enemyController.enemyData.fleeDistance)
        {
            Vector3 fleeDirection = GetFleeDirection(playerTransform);
            controller.movement.MoveInDirection(fleeDirection * enemyController.enemyData.movementSpeed * Time.deltaTime);
        }
        else
        {
            // If the ghost is far enough away, return to patrol state (don't reset ensnare progress; maybe decrement it slowly)
            enemyController.ChangeState(new PatrolState(enemyController, enemyController.enemyData));
        }
    }
    public Vector3 GetFleeDirection(Transform playerTransform)
    {
        Vector3 fleeDirection = controller.transform.position - playerTransform.position;

        // Flatten to the XZ plane to prevent ghosts from flying upward/downward
        fleeDirection.y = 0f;

        return fleeDirection.normalized;
    }
    private bool CheckEnsnareComplete()
    {
        return enemyController.ensnareProgress >= enemyController.enemyData.ensnaredValue;
    }

    /// <summary>
    /// Generates a random point within the patrol radius around the patrol center.
    /// Loops until a point is generated outside a certain radius around the player to avoid moving too close to the player
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = new Vector3();
        do
        {
            float angle = Random.Range(0f, 360f);
            float x = patrolCenter.x + enemyData.patrolRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = patrolCenter.z + enemyData.patrolRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            //float y = patrolCenter.y + Random.Range(0f, enemyData.maxPatrolHeight) + 0.5f; // TODO: Ground check to make sure we are spawning above ground
            float y = patrolCenter.y + Random.Range(playerPosition.position.y, playerPosition.position.y * 1.5f) + 0.5f; // Random height based on player position to avoid spawning underground
            randomPoint = new Vector3(x, y, z);
        } while (IsPointInPlayerRadius(randomPoint)); // Keep generating until a valid point is found

        return randomPoint;
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
