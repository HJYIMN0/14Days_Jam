using UnityEngine;
public abstract class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private bool hasMaxHpOnStart = true;

    private int _currentHealth;
    public int CurrentHealth => _currentHealth;
    private void Start()
    {
        if (hasMaxHpOnStart) _currentHealth = maxHealth;
    }
    public virtual void TakeDamage(int damageAmount)
    {
        if (isInvincible) return;

        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    protected abstract void Die();
}