using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float timeLimit = 5f;
    [SerializeField] private WeaponStateManager weaponStateManager;

    private float timer;
    public float TimeRemaining => timer;

    private void Start()
    {
        weaponStateManager.OnWeaponPickup += ResetTimer;
        timer = timeLimit;
        _timerText.text = timer.ToString();
    }
   
    private void FixedUpdate()
    {
        if (!weaponStateManager.HasWeapon)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                Debug.Log("Time's up! Player lost the weapon.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
