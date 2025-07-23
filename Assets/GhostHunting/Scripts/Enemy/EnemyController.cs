using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    private EnemyState currentState;
    public EnemyMovement movement;

    public void Initialize(EnemyData data)
    {
        enemyData = data;
        // Generate Name & Set Name

        // Set Movement Settings
        movement = GetComponent<EnemyMovement>();
        if (movement == null)
        {
            Debug.LogError("EnemyMovement component is missing on " + gameObject.name);
        }
    }
   
    void Start()
    {
        ChangeState(new PatrolState(this, enemyData));
    }

    void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }
}
