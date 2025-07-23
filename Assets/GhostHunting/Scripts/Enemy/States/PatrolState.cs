using UnityEngine;

public class PatrolState : EnemyState
{
    private Vector3 targetPosition;
    private EnemyData enemyData;
    public PatrolState(EnemyController controller, EnemyData data) : base(controller) 
    {
        this.enemyData = data;
    }

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
        float x = enemyData.patrolCenter.x + enemyData.patrolRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = enemyData.patrolCenter.z + enemyData.patrolRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
        float y = enemyData.patrolCenter.y + Random.Range(0f, enemyData.maxPatrolHeight) + 0.5f; // TODO: Ground check to make sure we are spawning above ground
        return new Vector3(x, y, z);
    }
}
