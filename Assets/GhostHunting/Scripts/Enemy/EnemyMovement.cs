using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 200f;

    public void MoveTo(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * movementSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    //[Header("Enemy Settings")]
    //private EnemyController enemyController;

    //[Header("Enemy Movement Settings")]
    //[SerializeField]
    //public float movementSpeed = 5f;
    //public float rotationSpeed = 200f;

    //[Header("Waiting Settings")]
    //[SerializeField]
    //public float waitTime = 1f;
    //private Vector3 targetPosition;
    //private bool isWaiting = false;

    //[Header("Debug Target Point")]
    //[SerializeField]
    //public float spawnRadius = 10f;
    //public float maxSpawnHeight = 1f;
    //private float heightOffset = 0.5f;
    //Vector3 radiusCenter = new Vector3(0, 0, 0);

    //void Start()
    //{
    //    enemyController = GetComponent<EnemyController>();
    //    targetPosition = GetPositionWithinRadius(spawnRadius, radiusCenter);
    //}

    //void Update()
    //{
    //    if (!isWaiting)
    //    {
    //        MoveToTarget(targetPosition);

    //        if (Vector3.Distance(transform.position, targetPosition) < 1.5f)
    //        {
    //            StartCoroutine(WaitAndSetNewTarget());
    //        }
    //    }
    //}

    //private IEnumerator WaitAndSetNewTarget()
    //{
    //    isWaiting = true;
    //    yield return new WaitForSeconds(waitTime);
    //    targetPosition = GetPositionWithinRadius(spawnRadius, radiusCenter);
    //    isWaiting = false;
    //}

    //void MoveToTarget(Vector3 targetPoint)
    //{
    //    // Flatten the Y axis to keep movement and direction horizontal
    //    Vector3 direction = (targetPoint - transform.position).normalized;

    //    // Move the enemy on XZ plane
    //    Vector3 move = direction * movementSpeed * Time.deltaTime;
    //    transform.position += move;

    //    // Rotate only on Y-axis to face movement direction
    //    if (direction != Vector3.zero)
    //    {
    //        Quaternion targetRotation = Quaternion.LookRotation(direction);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //    }
    //}

    //// Helper Functions
    //private Vector3 GetPositionWithinRadius(float radius, Vector3 center)
    //{
    //    float angle = Random.Range(0f, 360f);
    //    float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
    //    float z = center.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
    //    float y = center.y + Random.Range(0f, maxSpawnHeight) + heightOffset;          // TODO: Ground check to make sure we are spawning above ground
    //    return new Vector3(x, y, z);
    //}

}
