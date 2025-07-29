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

    [SerializeField]
    [Range(1, 10)]
    public float ensnaredValue = 5;
    [Range(10, 50)]
    public int fleeDistance = 20; // Distance at which the enemy stops fleeing from the player

    // Movement Settings
    [SerializeField]
    [Range(2, 10)]
    public float patrolRadius = 5;
    [Range(1, 3)]
    public float movementSpeed = 2;
    [Range(150, 250)]
    public float rotationSpeed = 200;
    [Range(0.5f, 3.5f)]
    public float waitTime = 0;

    // Enemy Prefab
    [SerializeField]
    public GameObject enemyPrefab;      // Basic prefab of the enemy w/variant specific material (model)           
}
