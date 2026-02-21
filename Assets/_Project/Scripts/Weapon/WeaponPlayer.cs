using System;
using UnityEngine;

public class WeaponPlayer : MonoBehaviour
{
    [SerializeField] private PlayerAttackController attackController;
    [SerializeField] private Vector3 offset;

    public Action<bool> OnWeaponActiveStateChanged;

    private void Start()
    {
        // Allineo lo stato iniziale UNA VOLTA
        bool hasWeapon = attackController.HasWeapon();
        gameObject.SetActive(hasWeapon);
    }

    private void Update()
    {
        // Se l’arma è attiva, segue il player
        if (gameObject.activeSelf)
        {
            transform.position = attackController.transform.position + offset;
        }
    }

    // Questo metodo verrà chiamato SOLO quando cambia stato arma
    public void SetWeaponActive(bool isActive)
    {
        if (gameObject.activeSelf == isActive)
            return; // Evita chiamate inutili

        gameObject.SetActive(isActive);
        OnWeaponActiveStateChanged?.Invoke(isActive);
    }
}