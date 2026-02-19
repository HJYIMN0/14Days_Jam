using System;
using UnityEngine;

public class PlayerColliderManager : MonoBehaviour
{

    public bool HasWeapon { get; private set; }
    public Action OnWeaponPickup;

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
}
