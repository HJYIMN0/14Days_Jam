using System;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private GameObject weaponThrow;
    [SerializeField] private GameObject weaponPlayer;
    [SerializeField] private float attackForce_Fast = 10f;
    [SerializeField] private float attackForce_Strong = 10f;
    [SerializeField] private AttackType leftMouseButtonAttackType = AttackType.Fast;
    [SerializeField] private AttackType rightMouseButtonAttackType = AttackType.Strong;
    [SerializeField] private ForceMode2D attackForceMode_Fast = ForceMode2D.Impulse;
    [SerializeField] private ForceMode2D attackForceMode_Strong = ForceMode2D.Impulse;
    [SerializeField] private AttackDirection standardAttackDirection = AttackDirection.Up;

    private PlayerColliderManager _colliderManager;

    private WeaponThrow_Strong weaponThrow_Strong => weaponThrow.GetComponent<WeaponThrow_Strong>();
    private WeaponThrow_Fast weaponThrow_Fast => weaponThrow.GetComponent<WeaponThrow_Fast>();

    private PlayerInputController _inputSystem;
    private PlayerMovementController _movementController;

    public Action OnAttack;

    public bool HasWeapon() => _colliderManager.HasWeapon && weaponPlayer.activeSelf;
    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInputController>();
        _colliderManager = GetComponent<PlayerColliderManager>();
        _movementController = GetComponent<PlayerMovementController>();
    }
    private void Start()
    {
        _colliderManager.OnWeaponPickup += () => SetWeapon(true);
        SetWeapon(true);
        // MODIFICA 1: aggiunta questa riga.
        // Motivo: SetWeapon(true) attivava il GameObject weaponPlayer ma non
        // aggiornava _colliderManager.HasWeapon, che rimaneva false (default bool).
        // CanAttack() controlla entrambi, quindi l'attacco era sempre bloccato.
        _colliderManager.SetWeapon(true);
    }

    public void SetWeapon(bool hasWeapon)
    {
        if (hasWeapon && weaponPlayer.activeSelf) return;
        if (!hasWeapon && !weaponPlayer.activeSelf) return;

        weaponPlayer.SetActive(hasWeapon);
        weaponThrow.SetActive(!hasWeapon);
    }

    private void Update()
    {
        if (_inputSystem.IsPlayerControlEnabled())
        {
            if (_inputSystem.InputSystem.Player.FastAttack.triggered)
            {
                Attack(leftMouseButtonAttackType);
            }
            if (_inputSystem.InputSystem.Player.StrongAttack.triggered)
            {
                Attack(rightMouseButtonAttackType);
            }
        }
    }

    public bool CanAttack() => _colliderManager.HasWeapon && weaponPlayer.activeSelf;

    private Vector2 CheckStandardAttackDirection(AttackDirection direction)
    {
        switch (direction)
        {
            case AttackDirection.Up:
                return Vector2.up;

            case AttackDirection.Down:
                return Vector2.down;

            case AttackDirection.Left:
                return Vector2.left;

            case AttackDirection.Right:
                return Vector2.right;

            default: return Vector2.up;
        }
    }

    private void Attack(AttackType attackType)
    {
        if (CanAttack())
        {
            weaponPlayer.SetActive(false);
            Vector2 attackDirection = _movementController.GetInputDirection();
            if (attackDirection == Vector2.zero) attackDirection = CheckStandardAttackDirection(standardAttackDirection);
            // Default attack direction if player is not moving

            // MODIFICA 2: spostato OnAttack?.Invoke() PRIMA di FastAttack/StrongAttack.
            // Motivo: OnAttack avvia AttackCoroutine in WeaponThrowController, che abilita
            // il collider. Se la forza veniva applicata prima, l'arma partiva già in volo
            // con il collider ancora spento, mancando le collisioni (visibile soprattutto
            // al primo lancio quando l'arma parte da ferma).
            _colliderManager.StartCoroutine(_colliderManager.AttackCoroutine(attackType));
            _colliderManager.SetWeapon(false);
            OnAttack?.Invoke();

            switch (attackType)
            {
                case AttackType.Fast:
                    FastAttack(attackDirection);
                    Debug.Log("Player attacks with weapon = fast!");
                    break;
                case AttackType.Strong:
                    StrongAttack(attackDirection);
                    Debug.Log("Player attacks with weapon = strong!");
                    break;
                default:
                    Debug.LogWarning("Unknown attack type!");
                    break;
            }
        }
        else
        {
            Debug.Log("Player cannot attack without a weapon!");
        }
    }
    private void FastAttack(Vector2 attackDir)
    {
        weaponThrow.SetActive(true);
        weaponThrow_Fast.enabled = true;
        weaponThrow_Strong.enabled = false;
        weaponThrow.GetComponent<Rigidbody2D>().AddForce(attackDir * attackForce_Fast, attackForceMode_Fast);
    }

    private void StrongAttack(Vector2 attackDir)
    {
        weaponThrow.SetActive(true);
        weaponThrow_Fast.enabled = false;
        weaponThrow_Strong.enabled = true;
        weaponThrow.GetComponent<Rigidbody2D>().AddForce(attackDir * attackForce_Strong, attackForceMode_Strong);
    }
}

public enum AttackDirection
{
    Up,
    Down,
    Left,
    Right
}

public enum AttackType
{
    Fast,
    Strong
}