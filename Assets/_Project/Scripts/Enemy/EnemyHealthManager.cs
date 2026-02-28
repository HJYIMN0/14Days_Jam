using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    protected override void Die()
    {
        Debug.Log("Enemy Died!");
        // Implement enemy death logic here, e.g., trigger death animation, drop loot, etc.
        Destroy(gameObject);
        LevelManager levelManager = GameObject.FindAnyObjectByType<LevelManager>();
        levelManager?.RemoveEnemyFromList(this.gameObject);
    }
}