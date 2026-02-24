using UnityEngine;
using System.Collections;
public class WeaponThrowController : MonoBehaviour
{
    [SerializeField] protected Transform playerTransform;

    [SerializeField] private float attackDuration = 01f;
    [SerializeField] private int damage = 1;

    public float AttackDuration => attackDuration;

    protected Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    protected virtual void OnEnable()
    {
        transform.position = playerTransform.position;
        StartCoroutine(ColliderWindowCoroutine());
    }
    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }
    private IEnumerator ColliderWindowCoroutine()
    {
        col.enabled = true;
        col.excludeLayers = LayerMask.GetMask("Player");

        yield return new WaitForSeconds(attackDuration);

        col.excludeLayers = 0;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<HealthManager>()?.TakeDamage(damage);
        }
    }
}