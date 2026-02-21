using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "ScriptableObjects/EnemySO", order = 1)]
public class EnemySO : ScriptableObject
{
    public int damage = 1;
    public int health = 1;
    public int DetectionRadius = 5;
    public float moveSpeed = 1f;
    public float timeToForgetPlayer = 5f;
    public LayerMask ThingsEnemyConsiderPlayer;

    public bool isFasterWhenChasing = false;
    public float chasingSpeedMultiplier = 1.5f;
}
