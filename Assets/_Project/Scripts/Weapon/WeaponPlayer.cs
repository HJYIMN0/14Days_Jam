using System;
using System.Collections;
using UnityEngine;

public class WeaponPlayer : MonoBehaviour
{
    [SerializeField] private PlayerAttackController attackController;
    [SerializeField] private Vector3 offset;
    public Vector3 Offset => offset;

    public Action<bool> OnWeaponActiveStateChanged;
    private void FixedUpdate()
    {
        if (attackController.HasWeapon() && this.gameObject.activeSelf)
        {
            transform.position = attackController.gameObject.transform.position + offset;
        }

        if (attackController.HasWeapon() && !this.gameObject.activeSelf)
        {
            Debug.LogWarning("Weapon_Player: Update: Player has weapon but Weapon_Player is not active.");
            this.gameObject.SetActive(true);
        }

        if (!attackController.HasWeapon() && this.gameObject.activeSelf)
        {
            Debug.LogWarning("Weapon_Player: Update: Player does not have weapon but Weapon_Player is active.");
            this.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        OnWeaponActiveStateChanged?.Invoke(true);
    }

    private void OnDisable()
    {
        OnWeaponActiveStateChanged?.Invoke(false);
    }
}