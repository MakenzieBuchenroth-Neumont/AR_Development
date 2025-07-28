using TMPro;
using UnityEngine;

public class EnsnaringState : EnemyState
{
    private EnemyController enemyController;
    private Transform playerTransform;
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
        FleeFromPlayer();
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
    private bool CheckEnsnareComplete()
    {
        return enemyController.ensnareProgress >= enemyController.enemyData.ensnaredValue;
    }
}
