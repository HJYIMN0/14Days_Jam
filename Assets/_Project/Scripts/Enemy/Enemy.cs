using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;

    // MODIFICA: routePos ora usa List<Transform> invece di List<Vector2>, come richiesto.
    // _startPos rimane Vector2 ed è garantita all'indice 0 di _resolvedRoute (lista interna privata).
    [SerializeField] private List<Transform> routePos;
    private List<Vector2> _resolvedRoute;

    private int currentRoute;

    public bool isIntrestedInPlayer = false;
    private EnemyState _currentEmenyState = EnemyState.Idle;
    public EnemySO EnemySO => enemySO;

    private bool _isCoroutineRunning = false;
    private Vector2 _startPos;

    private Rigidbody2D _rb;

    private GameObject _playerGo;
    public GameObject PlayerGo => _playerGo;

    public void SetPlayerGo(GameObject playerGo) => _playerGo = playerGo;

    private void OnDrawGizmos()
    {
        if (enemySO == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemySO.DetectionRadius);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (enemySO.hasRoute && routePos.Count <= 0)
        {
            Debug.LogWarning($"If this enemy ({gameObject.name}) was supposed to follow a path, you forgot to assign it");
            Debug.Log($"Assigning hasPath = false to {enemySO}");
        }
    }

    private void Start()
    {
        _startPos = transform.position;
        currentRoute = 0;

        // MODIFICA: Costruzione di _resolvedRoute con _startPos (Vector2) forzata
        // all'indice 0, seguita dalle posizioni dei Transform assegnati in inspector.
        _resolvedRoute = new List<Vector2>();
        _resolvedRoute.Add(_startPos);
        foreach (Transform t in routePos)
        {
            if (t != null)
                _resolvedRoute.Add(t.position);
            else
                Debug.LogWarning($"[{gameObject.name}] Un waypoint in routePos è null e verrà ignorato.");
        }
    }

    protected virtual void FixedUpdate()
    {
        EvaluateEnemyBehaviour(_currentEmenyState);
    }

    protected virtual void EvaluateEnemyBehaviour(EnemyState enemyState)
    {
        bool isPlayerDetected = IsPlayerInDetectionRadius();

        switch (enemyState)
        {
            case EnemyState.Idle:
                if (isPlayerDetected)
                {
                    SetEnemyState(EnemyState.Chasing);
                }
                // CORREZIONE BUG PATHING: hasRoute ora ha la priorità rispetto al ritorno a
                // _startPos. In precedenza Distance(_startPos) > 0.5f veniva valutato prima,
                // interrompendo la pattuglia ogni volta che il nemico era lontano dall'origine.
                else if (enemySO.hasRoute)
                {
                    // CORREZIONE BUG PATHING: il controllo waypoint e il movimento sono ora
                    // continui ogni FixedUpdate. In precedenza Move() veniva chiamato solo nel
                    // singolo frame in cui distance < 0.5f, causando lo stop immediato tra un
                    // waypoint e l'altro.
                    if (Vector2.Distance(transform.position, _resolvedRoute[currentRoute]) < 0.5f)
                    {
                        currentRoute++;
                        if (currentRoute >= _resolvedRoute.Count) currentRoute = 0;
                    }
                    Move(_resolvedRoute[currentRoute]);
                    RotateTowards(_resolvedRoute[currentRoute], enemySO.rotationSpeed);
                }
                else if (Vector2.Distance(transform.position, _startPos) > 0.5f)
                {
                    Move(_startPos);
                    RotateTowards(_startPos, enemySO.rotationSpeed);
                }
                else
                {
                    _rb.linearVelocity = Vector2.zero;
                }
                break;

            case EnemyState.Chasing:
                currentRoute = 0;
                if (!isPlayerDetected && !isIntrestedInPlayer)
                {
                    SetEnemyState(EnemyState.Idle);
                    _rb.linearVelocity = Vector2.zero;
                    return;
                }
                else if (!isPlayerDetected && isIntrestedInPlayer)
                {
                    StartCoroutine(DiminishEnemyInterest(enemySO.timeToForgetPlayer));
                }
                if (_playerGo == null)
                {
                    _playerGo = GameObject.FindGameObjectWithTag("Player");
                }
                Move(_playerGo.transform.position);
                RotateTowards(_playerGo.transform.position, enemySO.rotationSpeed);
                break;

            case EnemyState.Attacking:
                // Implement attack logic
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
        Collider2D[] go = Physics2D.OverlapCircleAll((Vector2)transform.position, enemySO.DetectionRadius, enemySO.ThingsEnemyConsiderPlayer);
        if (go.Length > 0)
        {
            _playerGo = go[0].gameObject;
            isIntrestedInPlayer = true;
            return true;
        }
        return false;
    }

    public virtual void Move(Vector2 pos)
    {
        float speed = enemySO.moveSpeed;
        if (enemySO.isFasterWhenChasing && _currentEmenyState == EnemyState.Chasing)
        {
            speed *= enemySO.chasingSpeedMultiplier;
        }

        // N.B: linearVelocity è corretto se utilizzi Unity 6. Per versioni precedenti usa 'velocity'.
        _rb.linearVelocity = (pos - (Vector2)transform.position).normalized * speed;
    }

    public virtual IEnumerator DiminishEnemyInterest(float timeToForget)
    {
        if (_isCoroutineRunning) yield break;

        _isCoroutineRunning = true;
        yield return new WaitForSeconds(timeToForget);

        // Valuta nuovamente la presenza dopo l'attesa per non forzare la dimenticanza
        // se il player è rientrato nel raggio di prossimità.
        if (!IsPlayerInDetectionRadius())
        {
            isIntrestedInPlayer = false;
        }

        _isCoroutineRunning = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out PlayerHealthManager healthManager))
            {
                healthManager.TakeDamage(enemySO.damage);
            }
        }
    }

    public void RotateTowards(Vector3 dir, float rotationSpeed)
    {
        Vector2 direction = (dir - transform.position).normalized;

        // NOTA: Mantenuto il -90f essenziale se il tuo sprite guarda nativamente verso l'alto.
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}