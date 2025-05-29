using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction shortInteractAction;
    private InputAction longInteractAction;

    private Camera playerCamera;
    private float moveSpeed = 5f;  // 通常移動速度
    private float currentMoveSpeed;  // 現在の移動速度

    private float lookSpeedX = 1f;  // 横の視点移動速度
    private float lookSpeedY = 1f;  // 縦の視点移動速度
    private float minY = -80f;      // 縦の回転制限（下向き）
    private float maxY = 80f;       // 縦の回転制限（上向き）
    private float currentY = 0f;    // 現在のX回転角度

    private bool longPressTriggered = false;
    private IInteractable currentInteractTarget;

    public float interactRange = 2.5f; // インタラクト距離

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentMoveSpeed = moveSpeed;
    }

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerCamera = GetComponentInChildren<Camera>();

        var inputActions = playerInput.actions;

        moveAction = inputActions["Move"];
        lookAction = inputActions["Look"];
        shortInteractAction = inputActions["InteractShort"];
        longInteractAction = inputActions["InteractLong"];
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();

        shortInteractAction.started += OnInteractStarted;
        shortInteractAction.Enable();

        longInteractAction.performed += OnInteractPerformed;
        longInteractAction.canceled += OnInteractCanceled;
        longInteractAction.Enable();

    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();

        shortInteractAction.started -= OnInteractStarted;
        shortInteractAction.Disable();

        longInteractAction.performed -= OnInteractPerformed;
        longInteractAction.canceled -= OnInteractCanceled;
        longInteractAction.Disable();
    }

    private void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        MovePlayer(moveInput);
        LookAround(lookInput);

        // 距離チェックで自動キャンセル
        if (currentInteractTarget != null && ((MonoBehaviour)currentInteractTarget) != null)
        {
            float distance = Vector3.Distance(transform.position, ((MonoBehaviour)currentInteractTarget).transform.position);
            if (distance > 3f)
            {
                Debug.Log("距離が離れたためインタラクト解除");
                currentInteractTarget.CancelInteract();
                currentInteractTarget = null;
            }
        }
        else
        {
            // Destroyされたか無効化されているのでクリア
            currentInteractTarget = null;
        }
    }

    private void MovePlayer(Vector2 moveInput)
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime);
    }

    public void LookAround(Vector2 lookInput)
    {
        transform.Rotate(Vector3.up, lookInput.x * lookSpeedX);

        currentY -= lookInput.y * lookSpeedY;
        currentY = Mathf.Clamp(currentY, minY, maxY);
        Camera.main.transform.localRotation = Quaternion.Euler(currentY, 0, 0);
    }

    private void TryLongInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // 🔁 別のオブジェクトを見ていたら、一度キャンセル
                if (currentInteractTarget != null && currentInteractTarget != interactable)
                {
                    Debug.Log("🔁 異なるインタラクト対象に切り替えたため、旧キャンセル");
                    currentInteractTarget.CancelInteract();
                }

                currentInteractTarget = interactable; // 新しい対象に更新

                // オブジェクトごとの長押し処理
                if (interactable is FuelTank fuelTank)
                {
                    fuelTank.StartFillingExternally();
                }
                else if (interactable is FuelStove stove)
                {
                    stove.StartFillingExternally();
                }
                else if (interactable is FuelGenerator generator)
                {
                    generator.StartFillingExternally();
                }
                else if (interactable is Window window)
                {
                    var selected = InventoryManager.Instance.GetSelectedItem();
                    if (selected is ItemData_WoodenPlank plank)
                    {
                        window.StartBoardingExternally(plank);
                    }
                    else
                    {
                        Debug.Log("🪵 木の板が選択されていません");
                    }
                }
            }
        }
        else
        {
            // ヒットしてないときはキャンセル
            if (currentInteractTarget != null)
            {
                Debug.Log("🔁 長押し先が見つからなかったためキャンセル");
                currentInteractTarget.CancelInteract();
                currentInteractTarget = null;
            }
        }
    }

    private void TryCancelInteract()
    {
        Debug.Log("インタラクトキャンセル");

        if (currentInteractTarget != null)
        {
            currentInteractTarget.CancelInteract();
            currentInteractTarget = null;
        }
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        Debug.Log("▶ 単押し開始（押した瞬間）");
        longPressTriggered = false;

        // 単押し対象だけ判定（長押し対象はスキップ）
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // ① ドア処理：DoorControllerを持つ親を探す
            var door = hit.collider.GetComponentInParent<DoorController>();
            if (door != null)
            {
                door.Interact(); // ← ドアを開閉
                return; // ドアなら他のインタラクト処理はしない
            }

            // ② それ以外のインタラクト処理
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // キャンセル時にも使うので記録
                if (interactable is FuelStove stove)
                {
                    currentInteractTarget = stove;
                }
                else if (interactable is FuelGenerator generator)
                {
                    currentInteractTarget = generator;
                }

                // 単押し専用のインタラクト（FuelStoveなどは除外）
                if (!(interactable is FuelTank) &&
                    !(interactable is FuelStove) &&
                    !(interactable is FuelGenerator) &&
                    !(interactable is Window))
                {
                    Debug.Log("▶ 単押し対象にヒット → Interact実行");
                    interactable.Interact();
                }
            }
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        // 長押し成立（Hold設定時）
        Debug.Log("▶ 長押し成功！");

        longPressTriggered = true;
        TryLongInteract(); // 長押し対象専用の処理だけ
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("▶ キャンセルまたは離された");

        if (!longPressTriggered)
        {
            // ここではもう何もしない（Interactは押下開始時に完結）
            Debug.Log("▶ 単押しキャンセル");

            // 短押し/長押し機能を持っているオブジェクトはここでInteract
            if (currentInteractTarget is FuelStove stove)
            {
                stove.Interact(); // 燃料ストーブ 電源ON/OFF
            }
            else if(currentInteractTarget is FuelGenerator generator)
            {
                generator.Interact(); // 燃料発電機 電源ON/OFF
            }

            currentInteractTarget = null; // 念のため
        }
        else
        {
            Debug.Log("▶ 長押し完了後の離し");
            TryCancelInteract(); // ← ここは長押しのキャンセル処理のみ
        }
    }
}
