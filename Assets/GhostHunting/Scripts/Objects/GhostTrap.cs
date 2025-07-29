using System.Threading.Tasks;
using UnityEngine;

public class GhostTrap : MonoBehaviour
{
    internal enum E_TrapState
    {
        None,
        Thrown,
        Activated,
        Failed,
        Success
    }
   
    [Header("Trapping Settings")]
    [SerializeField] private float trapDuration = 5f; // Duration of how long the trapping will take
    private E_TrapState trapState = E_TrapState.None;
    private GameObject ghost;

    private void Start()
    {

    }

    private void Update()
    {
        switch (trapState)
        {
            case E_TrapState.None:
                // Do nothing and or initialize anything needed
                break;
            case E_TrapState.Thrown:
                // Check for when the trap hits the ground 
                // When the ground is hit, change the state to Activated
                if (CheckForGround())
                {
                    ActivateTrap();
                }
                break;
            case E_TrapState.Activated:
                // Check for the ghost within the trap radius
                // If a ghost is within the trap radius, attempt to trap
                // Move to failed if no ghost is found within the trap radius
                // Move to failed if the trapping fails
                // Move to success if the trapping is successful
                break;
            case E_TrapState.Failed:
                // Allow player to throw another trap
                // Find a way to hold a max number of retries
                // Delete the trap after a certain time
                break;
            case E_TrapState.Success:
                // Handle the success of trapping a ghost
                // Figure out how, where and when to add the ghost to the players's inventory
                // Possibly play a success animation or sound (try to suck the ghost into the trap, shake trap, then remove trap from the scene after handling players inventory)
                break;
            default:
                break;
        }
    }

    public void ThrowTrap(Vector3 position, GameObject ghostToCapture, Camera arCamera, float trapThrowForce)
    {
        trapState = E_TrapState.Thrown;
        ghost = ghostToCapture;
        Vector3 ghostPosition = ghostToCapture.transform.position;
        Vector3 target = new Vector3(ghostPosition.x, 0.01f, ghostPosition.z);

        // Distance and Direction to target
        Vector3 toTarget = target - position;
        float distance = toTarget.magnitude;

        // Get players look angle
        float lookAngle = Vector3.Angle(arCamera.transform.forward, Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized);

        // Customize arc height based on look angle
        float arcHeight = Mathf.Lerp(1.0f, 2.0f, lookAngle / 90f);

        // Calculate upward offset to simulate arc
        Vector3 upwardArc = Vector3.up * arcHeight;

        // Final throw direction
        Vector3 throwDirection = (toTarget.normalized + upwardArc).normalized;

        // Apply force
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(throwDirection * trapThrowForce, ForceMode.VelocityChange);
    }

    private bool CheckForGround()
    {
        if (EnemyManager.IsInitialized)
        {
            Vector3 ghostPos = EnemyManager.Instance.spawnedEnemy.transform.position;
            if (gameObject.transform.position.y <= ghostPos.y - 5.0f)
            {
                return true;
            }
        }
        return false;
    }
    private void ActivateTrap()
    {
        trapState = E_TrapState.Activated;
        if (EnemyManager.Instance != null)
        {
            EnemyController controller = EnemyManager.Instance.spawnedEnemy.GetComponent<EnemyController>();
            if (!controller)
            {
                Debug.LogError("EnemyController not found on spawned enemy.");
                return;
            }

            if (controller.AttemptCapture())
            {
                CaptureGhost(controller);
            }
            else
            {
                FailTrap(controller);
            }
        }
    }

    private void CaptureGhost(EnemyController _controller)
    {
        trapState = E_TrapState.Success;
        _controller.ChangeState(new CapturedState(_controller));
        DestroyTrapAfterDuration(2f);
    }

    private void FailTrap(EnemyController _controller)
    {
        trapState = E_TrapState.Failed;
        if (EnemyManager.Instance)
        {
            EnemyManager.Instance.EnemyEscaped();
        }
        _controller.ChangeState(new PatrolState(_controller, _controller.enemyData));
        DestroyTrapAfterDuration(2f);
    }

    private async void DestroyTrapAfterDuration(float duration)
    {
        await Task.Delay((int)(duration * 1000));
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor") && trapState != E_TrapState.Activated)
        {
            // Activate the trap when it hits the floor
            ActivateTrap();
        }
    }
}
