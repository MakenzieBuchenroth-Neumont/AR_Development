using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Scriptable Objects/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    [SerializeField]
    public EnemyData[] baseEnemies;
    public EnemyData[] allVarianceEnemies;      // All variance and void variance will be the same until we have more variance types
    public EnemyData[] voidVarianceEnemies;
    public EnemyData[] allEnemies;
}
