using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerControls Controls { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;

        Controls = new PlayerControls();
    }

    private void OnEnable() => Controls.Enable();
    private void OnDisable() => Controls.Disable();

    public void EnableUIControls()
    {
        Controls.UI.Enable();
        Controls.Player.Disable();
        Controls.Inventory.Disable();
    }

    public void EnableGameplayControls()
    {
        Controls.UI.Disable();
        Controls.Player.Enable();
        Controls.Inventory.Enable();
    }
}
