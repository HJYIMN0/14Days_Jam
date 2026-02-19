using System;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private GameObject Weapon;
    [SerializeField] private float attackForce = 10f;

    private PlayerInputController _inputSystem;
    private PlayerColliderManager _colliderManager;
    private PlayerMovementController _movementController;

    public Action OnAttack;
    private void Awake()
    {
        _inputSystem        = GetComponent<PlayerInputController>();
        _colliderManager    = GetComponent<PlayerColliderManager>();
        _movementController = GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        if (_inputSystem.IsPlayerControlEnabled())
        {
            if (_inputSystem.InputSystem.Player.Attack.triggered)
            {
                Attack();
            }
        }
    }

    public bool CanAttack() => _colliderManager.HasWeapon;

    private void Attack()
    {
        if (CanAttack())
        {
            // Implement attack logic here, e.g., instantiate a weapon, apply force, etc.
            Debug.Log("Player attacks with weapon!");
            // Example: Apply a force to the weapon in the direction the player is facing
            Vector2 attackDirection = _movementController.GetInputDirection();
            Weapon.GetComponent<Rigidbody2D>().AddForce(attackDirection * attackForce, ForceMode2D.Impulse);
            _colliderManager.SetWeapon(false);
            OnAttack?.Invoke();
        }
        else
        {
            Debug.Log("Player cannot attack without a weapon!");
        }
    }
}
