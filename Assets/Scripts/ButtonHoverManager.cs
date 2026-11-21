using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverManager : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float hoveredScale = 1.2f;
    [SerializeField] private float otherHoveredScale = 0.85f;

    [Header("Animation")]
    [SerializeField] private float tweenDuration = 0.2f;

    private ButtonHoverItem[] buttons;
    private ButtonHoverItem hoveredButton;

    private void Awake()
    {
        buttons = GetComponentsInChildren<ButtonHoverItem>();
    }

    public void SetHoveredButton(ButtonHoverItem button)
    {
        hoveredButton = button;
        UpdateButtonScales();
    }

    public void ClearHoveredButton(ButtonHoverItem button)
    {      
        if (hoveredButton != button)
            return;

        hoveredButton = null;
        UpdateButtonScales();
    }

    private void UpdateButtonScales()
    {
        foreach (ButtonHoverItem button in buttons)
        {
            float targetScale = normalScale;

            if (hoveredButton != null)
            {
                targetScale = button == hoveredButton
                    ? hoveredScale
                    : otherHoveredScale;
            }

            button.TweenScale(targetScale, tweenDuration);
        }
    }
}
