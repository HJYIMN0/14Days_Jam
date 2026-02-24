using UnityEngine;
[RequireComponent(typeof(Enemy))] 
public class EnemyDamageController : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    private float _timer;
    private void Awake()
    {
        if (_enemy == null)
        _enemy = GetComponent<Enemy>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            Debug.Log("Enemy hit the player!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _enemy.EnemySO.damageRadius);
    }

    private void Update()
    {
        if (IsPlayerInDamageRadius())
        {
            _timer += Time.deltaTime;
            if (_timer >= _enemy.EnemySO.timeForDamage)
            {
                _enemy.PlayerGo?.GetComponent<HealthManager>()?.TakeDamage(_enemy.EnemySO.damage);
                _timer = 0f;
            }
        }
        else if (_timer > 0f)
        {
            _timer = 0f;
        }
    }

    public bool IsPlayerInDamageRadius() => Physics2D.OverlapCircle(transform.position, _enemy.EnemySO.damageRadius, _enemy.EnemySO.ThingsEnemyConsiderPlayer);
    


}
