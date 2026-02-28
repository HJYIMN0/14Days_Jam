using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager: HealthManager
{
    protected override void Die()
    {
        Debug.Log("Player Died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
