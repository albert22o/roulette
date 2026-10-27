using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TableWinButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action Clicked, HoverEnter, HoverExit;

    [SerializeField] private OutlineController outlineController;
    [SerializeField] private ButtonAnimation3D buttonAnimation;

    [Header("Visual")]
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color activeColor = new Color(1f, 0.85f, 0.1f);   // жёлтый
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f); // серый

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP / HDRP
    private static readonly int ColorId = Shader.PropertyToID("_Color");         // Built-in
    private MaterialPropertyBlock propertyBlock;
    private RoundManager roundManager;
    private RoundStateManager roundStateManager;
    private SFXManager sfxManager;
    private bool isHovered;

    [Inject]
    public void Construct(RoundManager roundManager, RoundStateManager roundStateManager, SFXManager sFXManager)
    {
        this.roundManager = roundManager;
        this.roundStateManager = roundStateManager;
        this.sfxManager = sFXManager;
        roundStateManager.StateChanged += OnStateChanged;
    }

    private void Start()
    {
        UpdateColor(); // начальное состояние
    }

    private void OnDestroy()
    {
        if (roundStateManager != null)
            roundStateManager.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged(RoundStateManager.RoundState state)
    {
        UpdateColor();

        if (roundManager.AbleToWin)
        {
            if (isHovered)
            {
                HoverEnter?.Invoke();
                outlineController.ToggleMaterial(true);
            }
        }
        else if (isHovered)
        {
            HoverExit?.Invoke();
            outlineController.ToggleMaterial(false);
        }
    }

    private void UpdateColor()
    {
        if (buttonRenderer == null || roundManager == null) return;

        propertyBlock ??= new MaterialPropertyBlock();
        Color color = roundManager.AbleToWin ? activeColor : inactiveColor;

        buttonRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(BaseColorId, color);
        propertyBlock.SetColor(ColorId, color);
        buttonRenderer.SetPropertyBlock(propertyBlock);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (roundManager.AbleToWin)
        {
            sfxManager.Play("ButtonClick");
            Clicked?.Invoke();
            HoverEnter?.Invoke();
            outlineController.ToggleMaterial(false);
            buttonAnimation.Press();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (roundManager.AbleToWin)
        {
            HoverEnter?.Invoke();
            outlineController.ToggleMaterial(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (roundManager.AbleToWin)
        {
            HoverExit?.Invoke();
            outlineController.ToggleMaterial(false);
        }
    }
}