using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    [SerializeField] private Image reticleImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite interactSprite;

    [SerializeField] private Vector2 defaultSize = new Vector2(32f, 32f);
    [SerializeField] private Vector2 interactSize = new Vector2(48f, 48f);

    [SerializeField] private float smoothTime = 0.1f; // サイズ補間速度

    private RectTransform rectTransform;
    private Vector2 targetSize;
    private Vector2 currentVelocity;

    [SerializeField] private LayerMask interactLayerMask;
    [SerializeField] private PlayerMovement playerMovement;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        rectTransform = reticleImage.GetComponent<RectTransform>();
        targetSize = defaultSize;
        if (reticleImage != null)
            reticleImage.sprite = defaultSprite;
    }

    private void Update()
    {
        UpdateReticle();
        SmoothResize();
    }

    private void UpdateReticle()
    {
        if (reticleImage == null || mainCamera == null)
        {
            Debug.LogWarning("ReticleController: reticleImage または Camera が設定されていません！");
            return;
        }

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, playerMovement.interactRange, interactLayerMask))
        {
            if (hit.collider.GetComponentInParent<IInteractable>() != null)
            {
                reticleImage.sprite = interactSprite;
                targetSize = interactSize;
                return;
            }
        }

        // 何もヒットしなければ通常のスプライトに戻す
        reticleImage.sprite = defaultSprite;
        targetSize = defaultSize;
    }

    private void SmoothResize()
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = Vector2.SmoothDamp(
                rectTransform.sizeDelta, targetSize, ref currentVelocity, smoothTime
            );
        }
    }
}
