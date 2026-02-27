using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovementController : MonoBehaviour
{
    private Rigidbody2D _playerRb;
    private Vector2 _movementInput;
    private Vector2 _mousePosition; // world space
    private PlayerInputController _inputSystem;
    [SerializeField] private ControlScheme controlScheme = ControlScheme.Mouse;
    public ControlScheme ControlScheme => controlScheme;
    [SerializeField] private float moveSpeed = 5f;
    public Vector2 GetInputDirection() => _movementInput.normalized;
    public Vector2 GetMouseDirection() => (_mousePosition - (Vector2)transform.position).normalized;
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

            // MODIFICA: Mouse.current.position è in screen space (pixel).
            // transform.position è in world space.
            // Sottrarli direttamente produce una direzione completamente sbagliata.
            // ScreenToWorldPoint converte i pixel schermo in coordinate world.
            // La z deve essere la distanza della camera dal piano di gioco (z=0),
            // che per una camera 2D standard posizionata a z=-10 vale 10.
            Vector2 screenPos = Mouse.current.position.ReadValue();
            _mousePosition = Camera.main.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z)
            );
        }
    }
    private void FixedUpdate()
    {
        if (controlScheme == ControlScheme.Mouse)
        {
            // _mousePosition è ora in world space → la sottrazione è corretta
            Vector2 lookDirection = _mousePosition - (Vector2)transform.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
            _playerRb.MoveRotation(angle);
        }
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