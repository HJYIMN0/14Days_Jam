using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
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
        // Modifica: Esecuzione singola del calcolo fisico e memorizzazione del risultato
        bool isPlayerDetected = IsPlayerInDetectionRadius();

        switch (enemyState)
        {
            case EnemyState.Idle:
                if (isPlayerDetected)
                {
                    SetEnemyState(EnemyState.Chasing);
                }
                else if (Vector2.Distance(transform.position, _startPos) > 0.5f)
                {
                    Move(_startPos);
                    RotateTowards(_startPos, enemySO.rotationSpeed);
                }
                else
                {
                    // Modifica: Azzeramento della velocità per impedire il jittering sul posto
                    _rb.linearVelocity = Vector2.zero;
                }
                break;

            case EnemyState.Chasing:
                if (!isPlayerDetected && !isIntrestedInPlayer)
                {
                    SetEnemyState(EnemyState.Idle);
                    _rb.linearVelocity = Vector2.zero; // Ferma il nemico istantaneamente
                    return; // Interrompe il blocco per impedire l'esecuzione di Move() sottostante
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
            // Modifica: Verifica dell'esistenza del componente prima di richiamarlo per evitare NullReferenceException
            if (collision.gameObject.TryGetComponent(out PlayerHealthManager healthManager))
            {
                healthManager.TakeDamage(enemySO.damage);
            }
        }
    }

    public void RotateTowards(Vector3 dir, float rotationSpeed)
    {
        // Calcolo del vettore verso il punto bersaglio 'dir'
        Vector2 direction = (dir - transform.position).normalized;

        // Calcolo dell'angolo target. 
        // NOTA: Mantenuto il -90f essenziale se il tuo sprite guarda nativamente verso l'alto.
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Creazione del Quaternione bersaglio finale
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
