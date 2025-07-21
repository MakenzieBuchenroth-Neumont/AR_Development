using UnityEngine;

public class PatrolState : EnemyState
{
    private Vector3 targetPosition;
    public PatrolState(EnemyController controller) : base(controller) { }

    public override void OnEnter() 
    {
        targetPosition = GetRandomPoint();
    }

    public override void OnExit() { }

    public override void UpdateState()
    {
        if (targetPosition != null)
        {
            controller.movement.MoveTo(targetPosition);

            if (Vector3.Distance(controller.transform.position, targetPosition) < 0.5f)
            {
                // Reached Target Position
                targetPosition = GetRandomPoint();
            }
        }
    }

    private Vector3 GetRandomPoint()
    {
        float angle = Random.Range(0f, 360f);
        float x = controller.patrolCenter.x + controller.spawnRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = controller.patrolCenter.z + controller.spawnRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
        float y = controller.patrolCenter.y + Random.Range(0f, controller.maxSpawnHeight) + 0.5f; // TODO: Ground check to make sure we are spawning above ground
        return new Vector3(x, y, z);
    }
}
