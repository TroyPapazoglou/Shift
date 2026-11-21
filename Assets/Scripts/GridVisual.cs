
using PrimeTween;
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GridVisual : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected float originalAlpha;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalAlpha = spriteRenderer.color.a;
    }

    public virtual void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    public virtual void AnimateTo(Vector3 destination, float duration, Vector3? wrapExit, Vector3? wrapEntry, Action onComplete = null)
    {
        if (wrapExit.HasValue && wrapEntry.HasValue)
        {
            float halfDuration = duration * 0.5f;

            Tween.Position(transform, wrapExit.Value, halfDuration);

            Tween.Alpha(spriteRenderer, 0f, halfDuration)
                .OnComplete(() =>
                {
                    transform.position = wrapEntry.Value;

                    Tween.Position(transform, destination, halfDuration)
                        .OnComplete(() => onComplete?.Invoke());

                    Tween.Alpha(spriteRenderer, originalAlpha, halfDuration);
                });
        }
        else
        {
            Tween.Position(transform, destination, duration)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}
