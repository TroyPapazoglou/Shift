using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ButtonHoverManager manager;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        manager = GetComponentInParent<ButtonHoverManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager != null)
            manager.SetHoveredButton(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (manager != null)
            manager.ClearHoveredButton(this);
    }

    public void TweenScale(float multiplier, float duration)
    {
        Vector3 targetScale = originalScale * multiplier;

        Tween.StopAll(transform);

        if (Vector3.Distance(transform.localScale, targetScale) < 0.001f)
            return;

        Tween.Scale(
            transform,
            targetScale,
            duration,
            Ease.OutCubic
        );
    }
}
