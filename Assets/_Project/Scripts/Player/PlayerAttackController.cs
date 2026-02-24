using UnityEngine;
public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private WeaponStateManager weaponStateManager;
    [SerializeField] private PlayerColliderManager colliderManager;
    [SerializeField] private GameObject weaponThrowObject;

    [SerializeField] private float attackForce_Fast = 10f;
    [SerializeField] private float attackForce_Strong = 10f;
    [SerializeField] private AttackType leftMouseButtonAttackType = AttackType.Fast;
    [SerializeField] private AttackType rightMouseButtonAttackType = AttackType.Strong;
    [SerializeField] private ForceMode2D attackForceMode_Fast = ForceMode2D.Impulse;
    [SerializeField] private ForceMode2D attackForceMode_Strong = ForceMode2D.Impulse;
    [SerializeField] private AttackDirection standardAttackDirection = AttackDirection.Up;


    private WeaponThrow_Strong _weaponThrow_Strong;
    private WeaponThrow_Fast _weaponThrow_Fast;
    private Rigidbody2D _weaponRb;

    private PlayerInputController _inputSystem;
    private PlayerMovementController _movementController;

    private void Awake()
    {
        _inputSystem = GetComponent<PlayerInputController>();
        _movementController = GetComponent<PlayerMovementController>();

        _weaponThrow_Strong = weaponThrowObject.GetComponent<WeaponThrow_Strong>();
        _weaponThrow_Fast = weaponThrowObject.GetComponent<WeaponThrow_Fast>();
        _weaponRb = weaponThrowObject.GetComponent<Rigidbody2D>();
    }
    public bool HasWeapon() => weaponStateManager.HasWeapon;
    public bool CanAttack() => weaponStateManager.HasWeapon;

    private void Update()
    {
        if (!_inputSystem.IsPlayerControlEnabled()) return;

        if (_inputSystem.InputSystem.Player.FastAttack.triggered)
            TryAttack(leftMouseButtonAttackType);

        if (_inputSystem.InputSystem.Player.StrongAttack.triggered)
            TryAttack(rightMouseButtonAttackType);
    }


    private void TryAttack(AttackType attackType)
    {
        if (!CanAttack())
        {
            Debug.Log("Player cannot attack without a weapon!");
            return;
        }
        Vector2 direction = (_movementController.ControlScheme == ControlScheme.Mouse)
        ? _movementController.GetMouseDirection()
        : _movementController.GetInputDirection();

        if (direction == Vector2.zero && _movementController.ControlScheme == ControlScheme.WASD)
            direction = GetStandardDirection();

        // 1. Aggiorna lo stato: player non ha più l'arma, WeaponPlayer disattivato.
        weaponStateManager.ThrowWeapon();

        // 2. Attiva il tipo corretto e applica la forza.
        //    SetActive(true) su weaponThrowObject triggera OnEnable su WeaponThrowController,
        //    che posiziona l'arma e abilita il collider. Nessun evento necessario.
        switch (attackType)
        {
            case AttackType.Fast:
                LaunchFast(direction);
                break;
            case AttackType.Strong:
                LaunchStrong(direction);
                break;
        }

        // 3. Disabilita temporaneamente la collisione player/arma.
        colliderManager.StartAttackCooldown(attackType);
    }

    private void LaunchFast(Vector2 direction)
    {
        weaponThrowObject.SetActive(true);  // → OnEnable su WeaponThrowController
        _weaponThrow_Fast.enabled = true;
        _weaponThrow_Strong.enabled = false;
        _weaponRb.AddForce(direction * attackForce_Fast, attackForceMode_Fast);
        Debug.Log("Player attacks: Fast!");
    }

    private void LaunchStrong(Vector2 direction)
    {
        weaponThrowObject.SetActive(true);  // → OnEnable su WeaponThrowController
        _weaponThrow_Fast.enabled = false;
        _weaponThrow_Strong.enabled = true;
        _weaponRb.AddForce(direction * attackForce_Strong, attackForceMode_Strong);
        Debug.Log("Player attacks: Strong!");
    }

    // PRIMA: switch/case con return. Ora expression switch (C# 8), più conciso.
    private Vector2 GetStandardDirection()
    {
        return standardAttackDirection switch
        {
            AttackDirection.Up => Vector2.up,
            AttackDirection.Down => Vector2.down,
            AttackDirection.Left => Vector2.left,
            AttackDirection.Right => Vector2.right,
            _ => Vector2.up
        };
    }
}

public enum AttackDirection { Up, Down, Left, Right }
public enum AttackType { Fast, Strong }
public enum ControlScheme
{
    WASD,
    Mouse,
}