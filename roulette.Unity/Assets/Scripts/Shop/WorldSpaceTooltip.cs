using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WorldSpaceTooltip : MonoBehaviour
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
        Canvas.ForceUpdateCanvases();

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

        Vector2 baseScreenPoint = new Vector2(screenPoint.x, screenPoint.y);
        Vector2 desiredScreenPos = baseScreenPoint + (offset * parentCanvas.scaleFactor);

        Vector2 scaledPadding = screenPadding * parentCanvas.scaleFactor;

        float effectiveScaleX = parentCanvas.scaleFactor * rect.localScale.x;
        float effectiveScaleY = parentCanvas.scaleFactor * rect.localScale.y;

        float tooltipWidthInPixels = rect.rect.width * effectiveScaleX;
        float tooltipHeightInPixels = rect.rect.height * effectiveScaleY;

        float leftEdge = desiredScreenPos.x - (tooltipWidthInPixels * rect.pivot.x);
        float rightEdge = desiredScreenPos.x + (tooltipWidthInPixels * (1f - rect.pivot.x));
        float bottomEdge = desiredScreenPos.y - (tooltipHeightInPixels * rect.pivot.y);
        float topEdge = desiredScreenPos.y + (tooltipHeightInPixels * (1f - rect.pivot.y));

        if (leftEdge < scaledPadding.x)
        {
            desiredScreenPos.x += (scaledPadding.x - leftEdge);
        }
        else if (rightEdge > Screen.width - scaledPadding.x)
        {
            desiredScreenPos.x -= (rightEdge - (Screen.width - scaledPadding.x));
        }

        if (bottomEdge < scaledPadding.y)
        {
            desiredScreenPos.y += (scaledPadding.y - bottomEdge);
        }
        else if (topEdge > Screen.height - scaledPadding.y)
        {
            desiredScreenPos.y -= (topEdge - (Screen.height - scaledPadding.y));
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
