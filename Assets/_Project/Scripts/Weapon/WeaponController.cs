using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private PlayerColliderManager _colliderManager;
    [SerializeField] private PlayerAttackController _attackController;
    [SerializeField] private float attackDuration = 0.25f; // Duration for which the weapon collider is active during an attack
    public void SetColliderActive(bool isActive) => this.gameObject.GetComponent<Collider2D>().enabled = isActive;

    private void Start()
    {
        _colliderManager.OnWeaponPickup += HandleWeaponPickup;
        _attackController.OnAttack += HandleAttack;
    }

    private void FixedUpdate()
    {
        if (_colliderManager.HasWeapon)
        {
            transform.position = _colliderManager.gameObject.transform.position; // Follow the player's position when the weapon is picked up
        }
    }

    private void HandleAttack()
    {
        StartCoroutine(AttackCoroutine());
        SetColliderActive(true); // Enable the collider when the player attacks
    }

    private IEnumerator AttackCoroutine()
    {
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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        Debug.Log("Weapon collided with Enemy");
    //        // Implement logic to damage the enemy, play effects, etc.
    //    }
    //}
}