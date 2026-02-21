using UnityEngine;

public class PlayerHealthManager: HealthManager
{
    protected override void Die()
    {
        Debug.Log("Player Died!");
        // Implement player death logic here, e.g., trigger death animation, respawn, etc.
    }
}
