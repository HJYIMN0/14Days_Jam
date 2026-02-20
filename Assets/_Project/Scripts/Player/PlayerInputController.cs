using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public InputSystem_Actions InputSystem { get; private set; }
    public bool isInitialized { get; private set; } = false;
    public bool IsPlayerControlEnabled() => InputSystem.Player.enabled;

    private void Awake()
    {
        InputSystem = new InputSystem_Actions();
        isInitialized = true;
    }
    public void SetPlayerControl(bool isEnabled)
    {
        if (isEnabled)
        {
            InputSystem.Player.Enable();
        }
        else
        {
            InputSystem.Player.Disable();
        }
    }
    public void TogglePlayerControl()
    {
        if (IsPlayerControlEnabled())
            SetPlayerControl(false);

        else SetPlayerControl(true);

    }
}
