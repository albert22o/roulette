using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScreenSpaceTooltip : MonoBehaviour
{
    [Header("Position Settings")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 50f);
    [SerializeField] private Vector2 screenPadding = new Vector2(20f, 20f);

    [SerializeField]private RectTransform rect;
    private Canvas parentCanvas;
    private Camera mainCamera;

    private Vector3 currentTargetWorldPosition;

    [Inject]
    private void Construct(Camera camera)
    {
        mainCamera = camera;
    }

    private void Awake()
    {
        rect.pivot = new Vector2(rect.pivot.x, 0f);

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError($"[ShopTooltip] Объект {gameObject.name} должен находиться внутри Canvas!", this);
        }
    }

    private void LateUpdate()
    {
        UpdateTooltipPosition();
    }

    public void Show(Vector3 worldPosition)
    {
        currentTargetWorldPosition = worldPosition;
        UpdateTooltipPosition();
        gameObject.SetActive(true);
    }


    private void UpdateTooltipPosition()
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(currentTargetWorldPosition);
        if (screenPoint.z < 0)
        {
            gameObject.SetActive(false);
            return;
        }
        else if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        Vector2 desiredScreenPos = (Vector2)screenPoint + (offset * parentCanvas.scaleFactor);
        Vector2 tooltipSizeInPixels = Vector2.Scale(rect.rect.size, rect.lossyScale);

        float leftEdge = desiredScreenPos.x - (tooltipSizeInPixels.x * rect.pivot.x);
        float rightEdge = desiredScreenPos.x + (tooltipSizeInPixels.x * (1f - rect.pivot.x));
        float bottomEdge = desiredScreenPos.y - (tooltipSizeInPixels.y * rect.pivot.y);
        float topEdge = desiredScreenPos.y + (tooltipSizeInPixels.y * (1f - rect.pivot.y));

        Vector2 currentPadding = screenPadding * parentCanvas.scaleFactor;

        if (leftEdge < currentPadding.x)
        {
            desiredScreenPos.x += (currentPadding.x - leftEdge);
        }
        else if (rightEdge > Screen.width - currentPadding.x)
        {
            desiredScreenPos.x -= (rightEdge - (Screen.width - currentPadding.x));
        }

        if (bottomEdge < currentPadding.y)
        {
            desiredScreenPos.y += (currentPadding.y - bottomEdge);
        }
        else if (topEdge > Screen.height - currentPadding.y)
        {
            desiredScreenPos.y -= (topEdge - (Screen.height - currentPadding.y));
        }

        Camera uiCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            desiredScreenPos,
            uiCamera,
            out Vector2 localPoint))
        {
            rect.anchoredPosition = localPoint;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
