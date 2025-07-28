using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    public EnemyState currentState;
    public EnemyMovement movement;

    public int id = -1;
    public bool isCaptured = false;
    public bool isEnsnared = false;

    public float ensnareProgress = 0;

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

    #region Ensare & Capture Management
    public void BeginEnsare()
    {
        if (ensnareProgress < enemyData.ensnaredValue)
        {
            ChangeState(new EnsnaringState(this));
            //AttemptEnsnare(0.1f);
        }
        else
        {
            // The ghost is already ensnared, handle logic and move to ensnared state
        }
    }
    public void AttemptEnsnare(float progressValue)
    {
        if (!isEnsnared && !isCaptured)
        {
            ensnareProgress += progressValue;
        }
    }

    public void AttemptCapture()
    {

    }
    #endregion
}
