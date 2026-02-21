using UnityEngine;

public class WeaponThrow_Strong : WeaponThrowController
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float minSpeedToExplode = 0.5f;

    public bool HasExploded { get; private set; }

    private void Update()
    {
        if (rb.linearVelocity.magnitude <= minSpeedToExplode && !HasExploded)
        {
            Explode();
            HasExploded = true;
        }
    }
    private void Explode()
    {
        Debug.Log("WeaponThrow_Strong: Explode called!");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") || hitCollider.CompareTag("Player"))
            {
                HealthManager healthManager = hitCollider.GetComponent<HealthManager>();
                healthManager?.TakeDamage(Damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}