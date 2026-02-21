using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
    private bool _isIntrestedInPlayer = false;
    private EnemyState _currentEmenyState = EnemyState.Idle;
    public bool IsIntrestedInPlayer => _isIntrestedInPlayer;

    private bool _isCoroutineRunning = false;
    private Vector2 _startPos;

    private Rigidbody2D _rb;

    private GameObject _playerGo;
    private void OnDrawGizmos()
    {
        if (enemySO == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemySO.DetectionRadius);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        _startPos = transform.position;
    }
    protected virtual void FixedUpdate()
    {
        EvaluateEnemyBehaviour(_currentEmenyState);
    }

    protected virtual void EvaluateEnemyBehaviour(EnemyState enemyState)
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                if (IsPlayerInDetectionRadius())
                {
                    SetEnemyState(EnemyState.Chasing);
                }
                else if (Vector2.Distance(transform.position, _startPos) > 0.1f)
                {
                    Move(_startPos);
                }
                break;
            case EnemyState.Chasing:
                if (!IsPlayerInDetectionRadius() && !_isIntrestedInPlayer)
                {
                    SetEnemyState(EnemyState.Idle);
                }
                else if (!IsPlayerInDetectionRadius() && _isIntrestedInPlayer)
                {
                    StartCoroutine(DiminishEnemyInterest(enemySO.timeToForgetPlayer));
                }
                if (_playerGo == null) _playerGo = GameObject.FindGameObjectWithTag("Player");
                Move(_playerGo.transform.position);
                break;
            case EnemyState.Attacking:
                // Implement attack logic and transition conditions here
                break;
        }
    }

    public virtual void SetEnemyState(EnemyState newState)
    {
        if (_currentEmenyState != newState)
        {
            _currentEmenyState = newState;
        }
    }

    public virtual bool IsPlayerInDetectionRadius()
    {
        Collider2D [] go = Physics2D.OverlapCircleAll((Vector2)transform.position, enemySO.DetectionRadius, enemySO.ThingsEnemyConsiderPlayer);
        if (go.Length > 0)
        {
            _playerGo = go[0].gameObject;
            Debug.Log("Player Detected!");
            _isIntrestedInPlayer = true;
            return true;
        }
        else return false;
      
    }
    public virtual void Move(Vector2 pos)
    {
        float speed = enemySO.moveSpeed;
        if (enemySO.isFasterWhenChasing && _currentEmenyState == EnemyState.Chasing)
        {
            speed *= enemySO.chasingSpeedMultiplier;
        }
        _rb.linearVelocity = (pos - (Vector2)transform.position).normalized * speed;
    }

    public virtual IEnumerator DiminishEnemyInterest(float timeToForget)
    {
        if (_isCoroutineRunning) yield break; // Exit if the coroutine is already running
        _isCoroutineRunning = true; // Set the flag to indicate the coroutine is running
        yield return new WaitForSeconds(timeToForget);
        if (!IsPlayerInDetectionRadius()) _isIntrestedInPlayer = false;
        _isCoroutineRunning = false; // Reset the flag after the coroutine finishes
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            player.GetComponent<PlayerHealthManager>().TakeDamage(enemySO.damage);

        }
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}
