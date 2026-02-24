using UnityEngine;

/// <summary>
/// REFACTORING COMPLETO.
///
/// PRIMA:
/// - Aveva OnWeaponActiveStateChanged (Action<bool>) → rimosso, non usato in modo rilevante.
/// - Aveva SetWeaponActive(bool) con logica di guard → rimosso, WeaponStateManager
///   gestisce direttamente weaponPlayerObject.SetActive().
/// - Start() chiamava attackController.HasWeapon() per decidere lo stato iniziale
///   → rimosso, WeaponStateManager gestisce lo stato iniziale in Awake().
/// - Referenziava PlayerAttackController solo per usarne il Transform → sostituito
///   con una diretta referenza al Transform del player, più chiara e meno accoppiata.
///
/// ORA:
/// - Responsabilità unica: seguire la posizione del player con un offset.
/// - L'attivazione/disattivazione è gestita interamente da WeaponStateManager.
/// - Zero logica di stato, zero eventi.
/// </summary>
public class WeaponPlayer : MonoBehaviour
{
    // PRIMA: [SerializeField] private PlayerAttackController attackController;
    // usato SOLO per attackController.transform.position → referenza diretta più pulita.
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        transform.position = playerTransform.position + offset;
    }
}