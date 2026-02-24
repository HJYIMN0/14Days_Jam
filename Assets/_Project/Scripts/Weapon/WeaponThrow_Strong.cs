using UnityEngine;
public class WeaponThrow_Strong : WeaponThrowController
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float minSpeedToExplode = 0.5f;

    public bool HasExploded { get; private set; }
    private bool _hasBeenLaunched;

    protected override void OnEnable()
    {
        base.OnEnable();
        // PRIMA: mancava → HasExploded rimaneva true dopo il primo lancio.
        HasExploded = false;
        _hasBeenLaunched = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Update()
    {
        if (!_hasBeenLaunched)
        {
            if (rb.linearVelocity.magnitude > minSpeedToExplode)
                _hasBeenLaunched = true;
            return;
        }

        if (!HasExploded && rb.linearVelocity.magnitude <= minSpeedToExplode)
        {
            HasExploded = true;
            Explode();
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
                hitCollider.GetComponent<HealthManager>()?.TakeDamage(1);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}