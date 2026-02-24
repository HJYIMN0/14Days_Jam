using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(LineRenderer))]
public class EnemyLineOfSightController : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _detectionDelay = 0.2f;

    private float _timeSinceLastCheck;

    public List<Transform> VisibleTargets { get; private set; } = new List<Transform>();

    private void Awake()
    {
        if (!_enemy.EnemySO.hasLineOfSight)
        {
            Debug.LogWarning("L'EnemySO associato a questo nemico non ha la proprietà 'hasLineOfSight' abilitata.");
            enabled = false;
            return;
        }

        if (_enemy == null)
            _enemy = GetComponent<Enemy>();

        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.loop = true;
        _lineRenderer.useWorldSpace = true;

        InitLineRenderer(_enemy.EnemySO.lineRendererPoints, _enemy.EnemySO.lineRendererWidth);
    }

    private void Update()
    {
        _timeSinceLastCheck += Time.deltaTime;
        if (_timeSinceLastCheck >= _detectionDelay)
        {
            _timeSinceLastCheck = 0f;
            FindVisibleTargets2D(
                _enemy.EnemySO.viewRadius,
                _enemy.EnemySO.viewAngle,
                _enemy.EnemySO.ThingsEnemyConsiderPlayer,
                _enemy.EnemySO.ThingsThatCanBlockEnemyView
            );

            // Modifica: Gestione corretta della delegazione di stato all'entità principale
            if (VisibleTargets.Count > 0)
            {
                if (_enemy.PlayerGo == null)
                {
                    _enemy.SetPlayerGo(VisibleTargets[0].gameObject);
                }

                // Forza l'interesse e lo stato di inseguimento
                _enemy.isIntrestedInPlayer = true;
                _enemy.SetEnemyState(EnemyState.Chasing);
            }
            else
            {
                // Errore risolto: La Coroutine deve essere avviata tramite il MonoBehaviour che la esegue
                _enemy.StartCoroutine(_enemy.DiminishEnemyInterest(_enemy.EnemySO.timeToForgetPlayer));
            }
        }
    }

    private void LateUpdate()
    {
        DrawCone2D(_enemy.EnemySO.lineRendererPoints, _enemy.EnemySO.viewAngle, _enemy.EnemySO.viewRadius);
    }

    private void InitLineRenderer(int positionCount, float lineWidth)
    {
        if (positionCount < 3) return;

        _lineRenderer.positionCount = positionCount;
        _lineRenderer.startWidth = lineWidth;
    }

    private void DrawCone2D(int positionCount, float viewAngle, float viewRadius)
    {
        if (positionCount < 3) return;

        _lineRenderer.SetPosition(0, transform.position);

        float startingAngle = -viewAngle / 2f;
        float angleStep = viewAngle / (positionCount - 2);

        for (int i = 1; i < positionCount; i++)
        {
            float currentAngle = startingAngle + angleStep * (i - 1);
            Quaternion rotation = Quaternion.Euler(0f, 0f, currentAngle);
            Vector3 direction = rotation * transform.up;
            Vector3 vertexPosition = transform.position + (direction * viewRadius);

            _lineRenderer.SetPosition(i, vertexPosition);
        }
    }

    private void FindVisibleTargets2D(float viewRadius, float viewAngle, LayerMask targetMask, LayerMask obstacleMask)
    {
        VisibleTargets.Clear();

        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            Transform target = targetsInRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.up, directionToTarget) < viewAngle / 2f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask);

                if (hit.collider == null)
                {
                    VisibleTargets.Add(target);
                }
            }
        }
    }
}