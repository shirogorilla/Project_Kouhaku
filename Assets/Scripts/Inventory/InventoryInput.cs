using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInput : MonoBehaviour
{
    private PlayerControls inputActions;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        inputActions = new PlayerControls();
        inventoryManager = GetComponent<InventoryManager>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Inventory.Scroll.performed += OnScroll;
        inputActions.Inventory.SelectLeft.performed += _ => inventoryManager.SelectPrevious();
        inputActions.Inventory.SelectRight.performed += _ => inventoryManager.SelectNext();
        inputActions.Inventory.UseItem.performed += _ => inventoryManager.UseSelectedItem();
        inputActions.Inventory.Drop.performed += _ => inventoryManager.DropSelectedItem();
    }

    private void OnDisable()
    {
        inputActions.Inventory.Scroll.performed -= OnScroll;
        inputActions.Inventory.SelectLeft.performed -= _ => inventoryManager.SelectPrevious();
        inputActions.Inventory.SelectRight.performed -= _ => inventoryManager.SelectNext();
        inputActions.Inventory.UseItem.performed -= _ => inventoryManager.UseSelectedItem();
        inputActions.Inventory.Drop.performed -= _ => inventoryManager.DropSelectedItem();
        inputActions.Disable();
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<float>();

        if (scroll > 0f)
            inventoryManager.SelectNext();
        else if (scroll < 0f)
            inventoryManager.SelectPrevious();
    }
}
