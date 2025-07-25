using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    // Enemy Settings
    [SerializeField]
    public string name;
    public EnemyType type;
    public EnemyVariant variant;
    public string desc;
    public Sprite image;

    // Movement Settings
    [SerializeField]
    public float patrolRadius;
    public float maxPatrolHeight;
    public Vector3 patrolCenter;
    public float movementSpeed;
    public float rotationSpeed;
    public float waitTime;

    // Enemy Prefab
    [SerializeField]
    public GameObject enemyPrefab;      // Basic prefab of the enemy w/variant specific material (model)           
}
