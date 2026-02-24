using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "ScriptableObjects/EnemySO", order = 1)]
public class EnemySO : ScriptableObject
{
    [Header("Basic stats")]
    public int damage = 1;
    public int health = 1;
    public float moveSpeed = 1f;
    public float rotationSpeed = 5f;

    [Header("Player detection stats")]
    public float DetectionRadius = 5;
    public float timeToForgetPlayer = 5f;
    public LayerMask ThingsEnemyConsiderPlayer;
    public LayerMask ThingsThatCanBlockEnemyView;

    [Header("Speed increments stats")]
    public bool isFasterWhenChasing = false;
    public float chasingSpeedMultiplier = 1.5f;

    [Header("Damage stats")]
    public float damageRadius = 1f;
    public float timeForDamage = 1f;

    [Header("LineRenderer stats")]
    public int lineRendererPoints = 10;
    public float viewAngle = 90f;
    public float viewRadius = 5f;
    public float lineRendererWidth = 0.1f;
    public bool hasLineOfSight = true;

}
