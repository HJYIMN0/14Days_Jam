using UnityEngine;
using System.Collections;

public class WeaponThrowController : MonoBehaviour
{
    [SerializeField] protected PlayerColliderManager _colliderManager;
    [SerializeField] protected PlayerAttackController attackController;
    [SerializeField] protected WeaponPlayer weapon_player;
    [SerializeField] private float attackDuration = 0.25f; // Duration for which the weapon collider is active during an attack
    public float AttackDuration => attackDuration;

    protected Rigidbody2D rb;
    public void SetColliderActive(bool isActive) => this.gameObject.GetComponent<Collider2D>().enabled = isActive;

    public Collider2D GetCollider() => this.gameObject.GetComponent<Collider2D>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _colliderManager.OnWeaponPickup += HandleWeaponPickup;
        attackController.OnAttack += HandleAttack;
        weapon_player.OnWeaponActiveStateChanged += HandleWeaponStatusChange;
    }

    private void HandleWeaponStatusChange(bool isActive)
    //false = player threw weapon, true = player picked up weapon
    {
        if (!isActive)
        {
            this.gameObject.SetActive(true);
        }
        else if (isActive && this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
    }
    private void HandleAttack()
    {
        StartCoroutine(AttackCoroutine());
        SetColliderActive(true); // Enable the collider when the player attacks
    }

    private IEnumerator AttackCoroutine()
    {
        if (!GetCollider().isActiveAndEnabled) GetCollider().enabled = true;

        GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("Player"); // Exclude the player layer from the collider
        yield return new WaitForSeconds(attackDuration); // Wait for the attack duration
        GetComponent<Collider2D>().excludeLayers = 0; // Reset the excluded layers after the attack

    }

    private void HandleWeaponPickup()
    {
        // Implement logic to handle weapon pickup, e.g., enable weapon visuals, update UI, etc.
        Debug.Log("Weapon picked up!");
        SetColliderActive(false); // Disable the collider after picking up the weapon
    }

    protected virtual void OnEnable()
    {
        AttackCoroutine();
        transform.position = attackController.gameObject.transform.position; // Ensure the thrown weapon starts at the player's position
    }
    protected virtual void OnDisable()
    {
        SetColliderActive(false); // Ensure the collider is disabled when the object is disabled
    }
}
