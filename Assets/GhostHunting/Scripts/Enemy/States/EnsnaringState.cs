using TMPro;
using UnityEngine;

public class EnsnaringState : EnemyState
{
    private EnemyController enemyController;
    private Transform playerTransform;

    private Vector3 targetPosition;
    private Vector3 patrolCenter;
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
    }
    public override void UpdateState()
    {
        // Check if the ensnaring progress is complete
        // If the enemy is ensnared, change to the ensnared state
        if (CheckEnsnareComplete())
        {
            enemyController.ChangeState(new EnsnaredState(enemyController));
            return; // Exit the update method
        }

        // Check how far away from the player the ghost is
        // If the ghost is far away from the player, move back to patrol state
        // Else, flee from the player
        //FleeFromPlayer();

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
        //        waitTimer = controller.enemyData.waitTime;
        //    }
        //}

    }

    public override void OnExit()
    {
        enemyController.isEnsnared = true;
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

    private Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = new Vector3();
        int attempts = 0;
        do
        {
            attempts++;
            float angle = Random.Range(0f, 360f);
            float x = patrolCenter.x + controller.enemyData.patrolRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = patrolCenter.z + controller.enemyData.patrolRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            //float y = patrolCenter.y + Random.Range(playerTransform.position.y, playerTransform.position.y * 1.5f) + 0.5f; // Random height based on player position to avoid spawning underground
            randomPoint = new Vector3(x, 0.15f, z);
            if (attempts > 15)
            {
                Debug.LogWarning("GetRandomPoint: Too many attempts to find a valid point, returning last generated point.");
                break; // Exit the loop after too many attempts
            }
        } while (!IsPointInValidRange(randomPoint)); // Keep generating until a valid point is found

        return randomPoint;
    }

    private bool IsPointInValidRange(Vector3 point)
    {
        if (playerTransform == null) return false; // If player position is not set, assume point is valid

        float distance = Vector3.Distance(point, playerTransform.position);

        float minDistance = 2f;
        float maxDistance = enemyController.enemyData.patrolRadius;

        return distance > minDistance && distance <= maxDistance;
    }
    private bool CheckEnsnareComplete()
    {
        return enemyController.ensnareProgress >= enemyController.enemyData.ensnaredValue;
    }
}
