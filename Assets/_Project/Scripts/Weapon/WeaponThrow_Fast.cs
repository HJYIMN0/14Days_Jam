using UnityEngine;

public class WeaponThrow_Fast : WeaponThrowController
{
    [SerializeField] private float returnSpeed_enemy = 15f;
    [SerializeField] private float returnSpeed_normal = 10f;
    [SerializeField] private float returnSpeed_weakener = 2f;
    [SerializeField] ForceMode2D _returnForceMode = ForceMode2D.Impulse;

    private float _returnSpeed;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            rb.AddForce((this.transform.position - attackController.gameObject.transform.position) * returnSpeed_enemy, _returnForceMode);
            collision.gameObject.GetComponent<HealthManager>()?.TakeDamage(Damage);
        }
        else
        {
            rb.AddForce((this.transform.position - attackController.gameObject.transform.position) * _returnSpeed, _returnForceMode);
        }

        _returnSpeed /= returnSpeed_weakener;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _returnSpeed = returnSpeed_normal;
    }
}