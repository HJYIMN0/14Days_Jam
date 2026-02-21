using UnityEngine;
using System.Collections;

public class WeaponThrowController : MonoBehaviour
{
    [SerializeField] protected PlayerColliderManager colliderManager;
    [SerializeField] protected PlayerAttackController attackController;
    [SerializeField] protected WeaponPlayer weaponPlayer;

    [SerializeField] private float attackDuration = 0.25f;

    public float AttackDuration => attackDuration;
    [SerializeField] private int damage = 1;
    public int Damage => damage;

    protected Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        colliderManager.OnWeaponPickup += HandleWeaponPickup;
        attackController.OnAttack += HandleAttack;

        // Stato iniziale coerente
        bool hasWeapon = attackController.HasWeapon();
        gameObject.SetActive(!hasWeapon);
    }

    public virtual void Update()
    {
        if (this.gameObject.activeSelf && !col.enabled)
        {
            col.enabled = true;
        }
    }
    private void HandleAttack()
    {
        // Il player lancia l’arma → disattivo weaponPlayer e attivo questa
        weaponPlayer.SetWeaponActive(false);
        gameObject.SetActive(true);

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        col.enabled = true;
        col.excludeLayers = LayerMask.GetMask("Player");

        yield return new WaitForSeconds(attackDuration);

        col.excludeLayers = 0;
    }

    private void HandleWeaponPickup()
    {
        // Il player riprende l’arma
        weaponPlayer.SetWeaponActive(true);
        gameObject.SetActive(false);
        col.enabled = false;
    }

    protected virtual void OnEnable()
    {
        transform.position = attackController.transform.position;
        col.enabled = true;
    }

    protected virtual void OnDisable()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyHealthManager>()?.TakeDamage(damage);
        }
    }
}