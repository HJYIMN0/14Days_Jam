using System.Collections;
using UnityEngine;

/// <summary>
/// REFACTORING COMPLETO.
///
/// PRIMA:
/// - Possedeva HasWeapon (bool) → rimosso, ora in WeaponStateManager.
/// - Aveva OnWeaponPickup (Action) → rimosso, ora gestito da WeaponStateManager.PickupWeapon().
/// - Aveva SetWeapon(bool) chiamato da PlayerAttackController → rimosso.
/// - Leggeva i componenti WeaponThrow_Fast e WeaponThrow_Strong per ricavare AttackDuration.
///
/// ORA:
/// - Responsabilità unica: gestire il collider del player durante il lancio.
/// - Rileva la collisione con l'arma → notifica WeaponStateManager.
/// - StartAttackCooldown(): disabilita temporaneamente la collisione player/arma
///   per evitare che l'arma colpisca subito il player che l'ha lanciata.
/// </summary>
public class PlayerColliderManager : MonoBehaviour
{
    [SerializeField] private WeaponStateManager weaponStateManager;

    // Mantenuti per leggere AttackDuration dei due tipi di lancio.
    [SerializeField] private WeaponThrow_Fast weaponThrow_Fast;
    [SerializeField] private WeaponThrow_Strong weaponThrow_Strong;

    public Collider2D PlayerCollider { get; private set; }

    private void Awake()
    {
        PlayerCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                Debug.Log("Player collided with Enemy");
                break;

            case "Weapon":
                Debug.Log("Player picked up Weapon");
                // PRIMA: HasWeapon = true; OnWeaponPickup?.Invoke();
                // ORA: tutto delegato a WeaponStateManager.
                weaponStateManager.PickupWeapon();
                break;

            default:
                Debug.Log("Player collided with: " + collision.gameObject.tag);
                break;
        }
    }

    /// <summary>
    /// PRIMA: AttackCoroutine(AttackType) — era public IEnumerator, avviato con StartCoroutine
    /// dall'esterno. Rinominato e incapsulato: ora si avvia da sola.
    /// Esclude il layer "Weapon" dal collider del player per la durata del lancio.
    /// </summary>
    public void StartAttackCooldown(AttackType attackType)
    {
        StartCoroutine(AttackCooldownCoroutine(attackType));
    }

    private IEnumerator AttackCooldownCoroutine(AttackType attackType)
    {
        PlayerCollider.excludeLayers = LayerMask.GetMask("Weapon");

        // PRIMA: switch/case con yield. Ora expression switch più conciso.
        float duration = attackType switch
        {
            AttackType.Fast => weaponThrow_Fast.AttackDuration,
            AttackType.Strong => weaponThrow_Strong.AttackDuration,
            _ => 0f
        };

        yield return new WaitForSeconds(duration);

        PlayerCollider.excludeLayers = 0;
    }
}