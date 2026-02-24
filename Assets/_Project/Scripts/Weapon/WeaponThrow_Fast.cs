using UnityEngine;

public class WeaponThrow_Fast : WeaponThrowController
{
    [SerializeField] private float returnSpeed_enemy = 15f;
    [SerializeField] private float returnSpeed_normal = 10f;
    [SerializeField] private float returnSpeed_weakener = 2f;
    [SerializeField] private ForceMode2D _returnForceMode = ForceMode2D.Impulse;

    private float _returnSpeed;

    protected override void OnEnable()
    {
        base.OnEnable();
        _returnSpeed = returnSpeed_normal;
    }

    // Nota tecnica: Se OnDisable esegue solo base.OnDisable(), può essere rimosso 
    // per mantenere lo script più pulito. Lo lascio per fedeltà alla tua stesura.
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        Vector2 returnDirection = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = Vector2.zero;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            rb.AddForce(returnDirection * returnSpeed_enemy, _returnForceMode);
        }
        else
        {
            rb.AddForce(returnDirection * _returnSpeed, _returnForceMode);

            // Riduce la velocità di ritorno ad ogni collisione non con nemici.
            _returnSpeed /= returnSpeed_weakener;
        }
    }
}