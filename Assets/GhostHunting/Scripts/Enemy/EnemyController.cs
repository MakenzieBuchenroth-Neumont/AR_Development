using UnityEngine;



public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    private EnemyState currentState;
    public EnemyMovement movement;

    [SerializeField]
    public float spawnRadius = 10f;
    public float maxSpawnHeight = 1f;
    public Vector3 patrolCenter = new Vector3(0, 0, 0);
    void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        if (movement == null)
        {
            Debug.LogError("EnemyMovement component is missing on " + gameObject.name);
        }
    }
    void Start()
    {
        ChangeState(new PatrolState(this));
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
