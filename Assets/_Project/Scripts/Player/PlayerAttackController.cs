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
    [SerializeField] private Vector2 standardAttackDirection = Vector2.up;

    private PlayerColliderManager _colliderManager;

    private WeaponThrow_Strong weaponThrow_Strong => weaponThrow.GetComponent<WeaponThrow_Strong>();
    private WeaponThrow_Fast weaponThrow_Fast => weaponThrow.GetComponent<WeaponThrow_Fast>();

    private PlayerInputController _inputSystem;
    private PlayerMovementController _movementController;

    public Action OnAttack;

    public bool HasWeapon() => _colliderManager.HasWeapon && weaponPlayer.activeSelf;
    private void Awake()
    {
        _inputSystem        = GetComponent<PlayerInputController>();
        _colliderManager    = GetComponent<PlayerColliderManager>();
        _movementController = GetComponent<PlayerMovementController>();
    }
    private void Start()
    {
        _colliderManager.OnWeaponPickup += () => SetWeapon(true);
        SetWeapon(true);
    }

    public void SetWeapon(bool hasWeapon)
    {
        if ( hasWeapon &&  weaponPlayer.activeSelf) return;
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

    private void Attack(AttackType attackType)
    {
        if (CanAttack())
        {
            weaponPlayer.SetActive(false);
            Vector2 attackDirection = _movementController.GetInputDirection();
            if (attackDirection == Vector2.zero) attackDirection = standardAttackDirection; // Default attack direction if player is not moving
            
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

            //_colliderManager.AttackCoroutine(attackType);
            _colliderManager.StartCoroutine(_colliderManager.AttackCoroutine(attackType));
            _colliderManager.SetWeapon(false);
            OnAttack?.Invoke();
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



public enum AttackType
{
    Fast,
    Strong
}