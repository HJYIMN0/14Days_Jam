using System;
using System.Collections;
using UnityEngine;

public class PlayerColliderManager : MonoBehaviour
{
    [SerializeField] private WeaponThrow_Strong weaponThrow_Strong;
    [SerializeField] private WeaponThrow_Fast weaponThrow_Fast;
    public Collider2D PlayerCollider { get; private set; }
    public bool HasWeapon { get; private set; }
    public Action OnWeaponPickup;

    private void Awake()
    {
        PlayerCollider = GetComponent<Collider2D>();
    }
    public void SetWeapon(bool hasWeapon)
    {
        if (hasWeapon != HasWeapon)
        {
            HasWeapon = hasWeapon;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                Debug.Log("Player collided with Enemy");
                break;
            case "Weapon":
                Debug.Log("Player collided with Weapon");
                HasWeapon = true;
                OnWeaponPickup?.Invoke();
                break;
            default:
                Debug.Log("Player collided with " + collision.gameObject.tag);
                break;
        }
    }

    public IEnumerator AttackCoroutine(AttackType attackType)
    {
        PlayerCollider.excludeLayers = LayerMask.GetMask("Weapon");
        switch (attackType)
        {
            case AttackType.Fast:
                yield return new WaitForSeconds(weaponThrow_Fast.AttackDuration);
                break;
            case AttackType.Strong:
                yield return new WaitForSeconds(weaponThrow_Strong.AttackDuration);
                break;
        }
        PlayerCollider.excludeLayers = 0;
    }
}