using System;
using UnityEngine;

/// <summary>
/// NUOVA CLASSE - Singola fonte di verità per lo stato dell'arma.
///
/// PROBLEMA ORIGINALE:
/// Lo stato "il player ha l'arma?" era distribuito in due posti indipendenti:
///   - PlayerColliderManager.HasWeapon  (bool)
///   - weaponPlayer.activeSelf          (bool)
/// CanAttack() li controllava entrambi. Bastava che uno dei due si
/// desincronizzasse (es. timing di Start()) per rompere il sistema.
/// Inoltre PlayerAttackController, WeaponThrowController e PlayerColliderManager
/// si occupavano tutti in parte di attivare/disattivare weaponPlayer e weaponThrow,
/// rendendo il flusso impossibile da seguire.
///
/// SOLUZIONE:
/// WeaponStateManager è l'unico script che può cambiare lo stato dell'arma.
/// Tutti gli altri script leggono HasWeapon da qui o chiamano i due metodi pubblici:
///   - ThrowWeapon()  → il player ha lanciato l'arma
///   - PickupWeapon() → il player ha ripreso l'arma
///
/// POSIZIONAMENTO: stesso GameObject del Player (insieme a PlayerAttackController
/// e PlayerColliderManager).
/// </summary>
public class WeaponStateManager : MonoBehaviour
{
    [SerializeField] private GameObject weaponPlayerObject;
    [SerializeField] private GameObject weaponThrowObject;

    public GameObject WeaponPlayerObj => weaponPlayerObject;
    public GameObject WeaponThrowObj => weaponThrowObject;

    /// <summary>
    /// Unica fonte di verità: se true, il player possiede l'arma.
    /// Rimpiazza PlayerColliderManager.HasWeapon e weaponPlayer.activeSelf.
    /// </summary>
    public bool HasWeapon { get; private set; }
    public Action OnWeaponPickup;

    private void Awake()
    {
        // Stato iniziale deterministico: player parte sempre con l'arma.
        // Prima questo veniva fatto in Start() in più script con logiche diverse,
        // causando race condition sull'ordine di esecuzione di Unity.
        SetWeaponState(playerHasWeapon: true);
    }

    /// <summary>
    /// Chiamato da PlayerAttackController quando il player lancia l'arma.
    /// Disattiva WeaponPlayer. WeaponThrow viene attivato DOPO da
    /// PlayerAttackController, perché dipende dal tipo di attacco (Fast/Strong).
    /// </summary>
    public void ThrowWeapon()
    {
        HasWeapon = false;
        weaponPlayerObject.SetActive(false);
        // weaponThrowObject è attivato da PlayerAttackController.Launch*()
        // subito dopo questa chiamata.
    }

    /// <summary>
    /// Chiamato da PlayerColliderManager quando il player tocca l'arma.
    /// Riattiva WeaponPlayer e disattiva WeaponThrow.
    /// </summary>
    public void PickupWeapon()
    {
        // SetActive(false) su weaponThrowObject triggera OnDisable su
        // WeaponThrowController, che si occupa di pulire il suo collider.
        SetWeaponState(playerHasWeapon: true);
        OnWeaponPickup?.Invoke();
    }

    private void SetWeaponState(bool playerHasWeapon)
    {
        HasWeapon = playerHasWeapon;
        weaponPlayerObject.SetActive(playerHasWeapon);
        weaponThrowObject.SetActive(!playerHasWeapon);
    }
}