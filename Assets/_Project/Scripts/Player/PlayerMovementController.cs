using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private Rigidbody2D _playerRb;
    private Vector2 _movementInput;
    private PlayerInputController _inputSystem;

    


    [SerializeField] private float moveSpeed = 5f;
    public Vector2 GetInputDirection() => _movementInput.normalized;



    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _inputSystem = GetComponent<PlayerInputController>();
    }

    private void Start()
    {
        _inputSystem.SetPlayerControl(true);
    }

    private void OnEnable()
    {
        if (_inputSystem.isInitialized)
        {
            _inputSystem.SetPlayerControl(true);
        }
    }

    private void OnDisable()
    {
        _inputSystem.SetPlayerControl(false);
    }

    private void Update()
    {
        if (_inputSystem.IsPlayerControlEnabled()) 
        {
            _movementInput = _inputSystem.InputSystem.Player.Move.ReadValue<Vector2>();
        } 
    }

    private void FixedUpdate()
    {
        if (IsMoving())
        {
            Move();
        }
        else
        {
            _playerRb.linearVelocity = new Vector2(0, 0);
        }
    }
    public bool IsMoving() => _movementInput.x != 0 || _movementInput.y != 0;

    private void Move()
    {
        Vector2 direction = _movementInput.normalized;
        _playerRb.linearVelocity = direction * moveSpeed;
    }

}
