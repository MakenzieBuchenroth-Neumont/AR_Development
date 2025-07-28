using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    private float movementSpeed = 5f;
    private float rotationSpeed = 200f;

    public void Initialize(float movementSpeed, float rotationSpeed)
    {
        this.movementSpeed = movementSpeed;
        this.rotationSpeed = rotationSpeed;
    }

    public void MoveTo(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * movementSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void MoveInDirection(Vector3 direction)
    {
        // Ensure direction is normalized (unit length)
        Vector3 normalizedDirection = direction.normalized;

        // Move ghost in the given direction
        transform.position += normalizedDirection * movementSpeed * Time.deltaTime;

        // Optionally rotate to face the movement direction on Y axis only
        if (normalizedDirection != Vector3.zero)
        {
            Vector3 flatDir = new Vector3(normalizedDirection.x, 0f, normalizedDirection.z);
            Quaternion targetRotation = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }

    public void RotateToPoint(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
