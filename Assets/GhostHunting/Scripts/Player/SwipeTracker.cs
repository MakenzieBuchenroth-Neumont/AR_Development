using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SwipeTracker : MonoBehaviour
{
    [Header("Swipe Detection Settings")]
    [SerializeField] private float minSwipeDistance = 50f;
    [SerializeField] private float maxSwipeDuration = 2f; // Max time (in seconds) to consider a swipe
    [SerializeField] private float circleThreshold = 0.85f; // 1 = perfect circle, lower = less circular

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isSwiping = false;
    private float swipeStartTime;

    // Real-time circle tracking
    [Header("Circle Tracking Settings")]
    [SerializeField] private float rotationThreshold = 300f; // Minimum angle change to count as a rotation
    [SerializeField] private int numberOfRays = 15;
    [SerializeField] private float maxRaycastDistance = 10f;
    [SerializeField] private float detectionRadius = 10f;
    private Vector2 centroid;
    private float accumulatedAngle = 0f;
    private float lastAngle = 0f;
    private bool centroidInitialized = false;
    private bool justInitializedAngle = false;

    [Header("Trap Settings")]
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private float trapThrowForce = 10f;
    [SerializeField] private Transform trapSpawnPoint;
    private bool trapSelected = false;
    public bool canThrowTrap = false;

    // Line Rendering
    [Header("Line Rendering Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform arOrigin;
    [SerializeField] private float trailDuration = 0.3f;
    private Camera arCamera;
    private List<Vector2> swipePath = new();
    private List<TrailPoint> trailPoints = new();
    private struct TrailPoint
    {
        public Vector3 position;
        public float timestamp;
    }

    // State/Ghost Variables
    //private EnemyManager enemyManager;
    [Header("Ghost Interaction Settings")]
    public bool canEnsnare { get; set; } =  false;
    public GameObject ghostToCapture;
    
    void Awake()
    {
        // Initialize enemy manager reference

        // Initialize line renderer
        if (lineRenderer == null)
        {
            return;
        }
        //mainCamera = Camera.main;
        arCamera = arOrigin.GetComponentInChildren<Camera>();
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.002f;
        lineRenderer.endWidth = 0.001f;
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.blue;
    }
    void Update()
    {
        if (Touchscreen.current != null)
        {
            HandleTouchInput();
        }
        else if (Mouse.current != null) // debug only
        {
            HandleMouseInput();
        }
    }

    #region Input Handling
    private void HandleTouchInput()
    {
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            BeginSwipe(touch.position.ReadValue());
        }

        if (isSwiping)
        {
            UpdateSwipe(touch.position.ReadValue());
        }

        if (touch.press.wasReleasedThisFrame)
        {
            EndSwipe();
        }
    }
    private void HandleMouseInput()
    {
        var mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            BeginSwipe(mouse.position.ReadValue());
        }

        if (isSwiping)
        {
            UpdateSwipe(mouse.position.ReadValue());
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            EndSwipe();
        }
    }
    #endregion

    #region Swipe Handling
    private void BeginSwipe(Vector2 position)
    {
        startTouchPosition = position;
        swipeStartTime = Time.time;
        isSwiping = true;
        swipePath.Clear();
        swipePath.Add(position);
        centroidInitialized = false;
        accumulatedAngle = 0f;
    }

    private void UpdateSwipe(Vector2 position)
    {
        currentTouchPosition = position;

        // Add points to path only if they are far enough from the last point
        if (swipePath.Count == 0 || Vector2.Distance(swipePath[^1], position) > 5f)
        {
            swipePath.Add(position);
            TrackRotationProgress(position);
            UpdateLine(position);
        }
    }

    private void EndSwipe()
    {
        isSwiping = false;

        float duration = Time.time - swipeStartTime;
        float swipeDistance = Vector2.Distance(currentTouchPosition, startTouchPosition);

        if (swipeDistance >= minSwipeDistance && duration <= maxSwipeDuration)
        {
            AnalyzeGesture();
        }

        ClearLine();
    }
    #endregion

    #region Gesture Analysis
    private void AnalyzeGesture()
    {
        if (!trapSelected || !canThrowTrap || swipePath.Count < 2) return;

        Vector2 start = swipePath[0];
        Vector2 end = swipePath[^1];
        Vector2 direction = (end - start).normalized;

        if (Vector2.Dot(direction, Vector2.up) > 0.8f)
        {
            ThrowTrap();
            trapSelected = false;
        }
    }

    /// <summary>
    /// Determines if the gesture forms a circular path using centroid and radius variance.
    /// </summary>
    /// <param name="path">A list of 2d Vector points</param>
    /// <returns>True if circular, else false</returns>
    private bool IsCircleGesture(List<Vector2> path)
    {
        if (path.Count < 5) return false; // Not enough points to form a circle

        // Find center
        Vector2 center = Vector2.zero;
        foreach (var point in path)
        {
            center += point;
        }
        center /= path.Count;

        // Calculate average radius and variance
        float totalRadius = 0f;
        float totalVariance = 0f;

        foreach (var point in path)
        {
            float radius = Vector2.Distance(center, point);
            totalRadius += radius;
        }

        float avgRadius = totalRadius / path.Count;

        foreach (var point in path)
        {
            float radius = Vector2.Distance(center, point);
            totalVariance += Mathf.Pow(radius - avgRadius, 2);
        }

        float variance = totalVariance / path.Count;
        float circularityScore = 1f - Mathf.Clamp01(variance / (avgRadius * avgRadius)); // Normalize variance

        Debug.Log($"Circularity Score: {circularityScore:F2}");

        return circularityScore >= circleThreshold;
    }
    /// <summary>
    /// Projects swipe points to polar coordinates around the centroid
    /// Tracks the total angular change as the user moves around the centroid
    /// Count a full circle when the angular change is roughly 360 degrees
    /// </summary>
    /// <param name="path">Path of 2d vector points</param>
    /// <returns>The number of full rotations made</returns>
    private int CountFullCircles(List<Vector2> path)
    {
        // Calculate centroid
        Vector2 center = Vector2.zero;
        foreach (var point in path)
        {
            center += point;
        }
        center /= path.Count;

        // Convert path to angles
        float totalAngle = 0f;
        float previousAngle = Mathf.Atan2(path[0].y - center.y, path[0].x - center.x);

        for (int i = 1; i < path.Count; i++)
        {
            float currentAngle = Mathf.Atan2(path[i].y - center.y, path[i].x - center.x);
            float deltaAngle = Mathf.DeltaAngle(previousAngle * Mathf.Rad2Deg, currentAngle * Mathf.Rad2Deg);

            totalAngle += deltaAngle;
            previousAngle = currentAngle;
        }

        float absTotalAngle = Mathf.Abs(totalAngle);
        int fullCircles = Mathf.FloorToInt(absTotalAngle / 360f);

        Debug.Log($"Total angle: {totalAngle:F2} degrees, Circles: {fullCircles}");
        return fullCircles;
    }
    #endregion

    #region Rotation Tracking
    private void TrackRotationProgress(Vector2 currentPos)
    {
        // Initialize centroid when we have enough points
        if (!centroidInitialized && swipePath.Count >= 2)
        {
            centroid = Vector2.zero;
            foreach (var point in swipePath)
            {
                centroid += point;
            }
            centroid /= swipePath.Count;

            lastAngle = Mathf.Atan2(swipePath[0].y - centroid.y, swipePath[0].x - centroid.x);
            centroidInitialized = true;
            justInitializedAngle = true;
        }

        if (!centroidInitialized) return;

        float currentAngle = Mathf.Atan2(currentPos.y - centroid.y, currentPos.x - centroid.x);
        if (justInitializedAngle)
        {
            // Skip the first angle calculation 
            lastAngle = currentAngle;
            justInitializedAngle = false;
            return;
        }

        float deltaAngle = Mathf.DeltaAngle(lastAngle * Mathf.Rad2Deg, currentAngle * Mathf.Rad2Deg);
        accumulatedAngle += deltaAngle;
        lastAngle = currentAngle;

        while (Mathf.Abs(accumulatedAngle) >= rotationThreshold)
        {
            accumulatedAngle -= 360f * Mathf.Sign(accumulatedAngle);
            OnCircleCompleted();
        }
    }

    private void OnCircleCompleted()
    {
        if (swipePath.Count < 2) return;

        Vector2 screenCenter = Vector2.zero;
        foreach (var point in swipePath)
        {
            screenCenter += point;
        }
        screenCenter /= swipePath.Count;

        // Convert to world direction
        //Ray centerRay = mainCamera.ScreenPointToRay(screenCenter);
        Ray centerRay = arCamera.ScreenPointToRay(screenCenter);
        
        Vector3 worldCenter = centerRay.origin + centerRay.direction * 5f; // pushed slightly forward
        Debug.DrawRay(centerRay.origin, centerRay.direction * detectionRadius, Color.green, 2f);

        // Estimate swipe radius on screen
        float avgScreenRadius = 0f;
        foreach (var point in swipePath)
        {
            avgScreenRadius += Vector2.Distance(point, screenCenter);
        }
        avgScreenRadius /= swipePath.Count;

        // Estimate cone angle based on screen radius
        float angleSpread = Mathf.Clamp(avgScreenRadius / Screen.width * 90f, 10f, 90f); // 10 to 90 degrees

        // Forward vector from camera through screen center
        Vector3 baseDirection = centerRay.direction.normalized;

        // Cast rays in an arc around the base direction
        Transform camTransform = arCamera.transform;
        Vector3 right = camTransform.right;

        for (int i = 0; i < numberOfRays; i++)
        {
            float t = (float)i / (numberOfRays - 1);
            float angleOffset = Mathf.Lerp(-angleSpread / 2f, angleSpread / 2f, t);

            // Rotate baseDirection around camera's up vector
            Quaternion rotation = Quaternion.AngleAxis(angleOffset, camTransform.up);
            Vector3 rayDirection = rotation * baseDirection;

            Ray ray = new Ray(camTransform.position, rayDirection);
            Debug.DrawRay(ray.origin, ray.direction * maxRaycastDistance, Color.red, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Handle ghost detection logic here
                    Debug.Log($"Ghost detected: {hit.collider.name} at {hit.point}");
                    HandleEnsare(hit.collider.gameObject);
                    break;
                }
            }
        }

        //Vector2 screenCenter = Vector2.zero;

        //foreach (var point in swipePath)
        //{
        //    screenCenter += point;
        //}
        //screenCenter /= swipePath.Count;

        //// Convert to world center using raycast from screen
        //Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        //Vector3 worldCenter = ray.origin + ray.direction * 5f; // 5 units into the world

        //// Draw the debug ray in the correct direction
        //Debug.DrawRay(ray.origin, ray.direction * detectionRadius, Color.red, 2f);

        //// Use raycast or OverlapSphere at the swipes world center
        //Collider[] hitColliders = Physics.OverlapSphere(worldCenter, detectionRadius);

        //foreach (var hit in hitColliders)
        //{
        //    if (hit.CompareTag("Enemy"))
        //    {
        //        // Handle ghost detection logic here
        //        Debug.Log($"Ghost detected: {hit.name} at {hit.transform.position}");
        //        HandleEnsare(hit.gameObject);
        //    }
        //}
    }
    #endregion

    #region Helper Functions
    private Vector3 EstimateSwipeCenter()
    {
        if (trailPoints.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (var point in trailPoints)
        {
            sum += point.position;
        }

        return sum / trailPoints.Count;
    }
    private void HandleEnsare(GameObject ghost)
    {
        if (ghost.TryGetComponent(out EnemyController controller))
        {
            if (controller.isCaptured)
            {
                // already captured, do nothing
            }
            else if (!controller.isEnsnared && controller.currentState is not EnsnaringState)
            {
                controller.BeginEnsare();
            }
            else if (controller.currentState is EnsnaringState)
            {
                controller.AttemptEnsnare(1f);
            }
        }
    }
    private void ThrowTrap()
    {
        if (trapPrefab == null || arCamera == null || ghostToCapture == null) return;

        Vector3 spawnPosition = trapSpawnPoint.position != null
            ? trapSpawnPoint.position
            : arCamera.transform.position + arCamera.transform.forward * 0.5f;

        GameObject trap = Instantiate(trapPrefab, spawnPosition, Quaternion.identity);
        if (trap.TryGetComponent<GhostTrap>(out GhostTrap ghostTrap))
        {
            ghostTrap.ThrowTrap(spawnPosition, ghostToCapture, arCamera);
        }
        else
        {
            Debug.LogError("GhostTrap component is missing on the trap prefab.");
            return;
        }

        if (trap.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Vector3 ghostPosition = ghostToCapture.transform.position;
            Vector3 target = new Vector3(ghostPosition.x, 0.01f, ghostPosition.z);

            // Distance and Direction to target
            Vector3 toTarget = target - spawnPosition;
            float distance = toTarget.magnitude;

            // Get players look angle
            float lookAngle = Vector3.Angle(arCamera.transform.forward, Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized);

            // Customize arc height based on look angle
            float arcHeight = Mathf.Lerp(1.0f, 4.0f, lookAngle / 90f);

            // Calculate upward offset to simulate arc
            Vector3 upwardArc = Vector3.up * arcHeight;

            // Final throw direction
            Vector3 throwDirection = (toTarget.normalized + upwardArc).normalized;

            // Apply force
            rb.AddForce(throwDirection * trapThrowForce, ForceMode.VelocityChange);
        }
    }
    public void SelectTrap()
    {
        trapSelected = true;
        canThrowTrap = true;
        Debug.Log("Trap selected. Swipe up to throw.");
    }
    #endregion

    #region Rendering
    private void UpdateLine(Vector2 screenPos)
    {
        Vector3 worldPos = arCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0.5f));

        if (trailPoints.Count == 0 || Vector3.Distance(worldPos, trailPoints[^1].position) > 0.01f)
        {
            trailPoints.Add(new TrailPoint
            {
                position = worldPos,
                timestamp = Time.time
            });
        }
        LateUpdate();
    }

    private void LateUpdate()
    {
        if (!isSwiping) return;

        float currentTime = Time.time;
        trailPoints.RemoveAll(p => currentTime - p.timestamp > trailDuration);

        lineRenderer.positionCount = trailPoints.Count;
        for (int i = 0; i < trailPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, trailPoints[i].position);
        }
    }

    private void ClearLine()
    {
        trailPoints.Clear();
        lineRenderer.positionCount = 0;
    }
    #endregion
}
