using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private WeaponStateManager _weaponStateManager;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float timeLimit = 5f;
    [SerializeField] private WeaponStateManager weaponStateManager;

    private float timer;
    public float TimeRemaining => timer;

    private void Start()
    {
        _weaponStateManager.OnWeaponPickup += ResetTimer;
    }
    private void Update()
    {
        if (!_weaponStateManager.HasWeapon)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                Debug.Log("Time's up! Player lost the weapon.");
            }
            _timerText.text = timer.ToString();
        }
    }

    private void ResetTimer()
    {
        timer = timeLimit;
        _timerText.text = timeLimit.ToString();
    }
}
