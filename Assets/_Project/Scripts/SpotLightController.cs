using UnityEngine;

public class SpotLightController : MonoBehaviour
{
    [SerializeField] private WeaponStateManager weaponStateManager;
    [SerializeField] private float zPos;

    private Transform _target;

    private void Start()
    {
        _target = weaponStateManager.WeaponPlayerObj.transform;
    }
    private void Update()
    {
        if (weaponStateManager.HasWeapon)
        {
            if (_target != weaponStateManager.WeaponPlayerObj && weaponStateManager.WeaponPlayerObj.activeSelf)
            {
                SwapTarget(weaponStateManager.WeaponPlayerObj.transform);
            }
        }
        else 
        {
            if (_target != weaponStateManager.WeaponThrowObj && weaponStateManager.WeaponThrowObj.activeSelf) 
            {
                SwapTarget(weaponStateManager.WeaponThrowObj.transform);
            }
        }
    }

    private void FixedUpdate()
    {
        FollowTarget(_target);
    }

    public void SwapTarget(Transform target)
    {
        if (_target != target) 
        {
            if (target.gameObject.activeSelf)
            {
                _target = target;
            }
        }
    }

    public void FollowTarget(Transform target)
    {
        Vector3 targetPosition = new Vector3(_target.position.x, _target.position.y, zPos);
        if (transform.position != targetPosition)
        {
            transform.position = targetPosition;
        }
    }
}
