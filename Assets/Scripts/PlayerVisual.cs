using PrimeTween;
using UnityEngine;

public class PlayerVisual : GridVisual
{
    private Tween bumpTween;

    [SerializeField] private int sortingOrderOffset = 1;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer.sortingOrder += sortingOrderOffset;
    }

    public void Bump(Vector3 restPosition, Vector2 direction, float distance, float duration)
    {
        if (bumpTween.isAlive)
            bumpTween.Stop();

        transform.position = new Vector3(restPosition.x, restPosition.y, transform.position.z);

        Vector3 start = transform.position;
        Vector3 offset = (Vector3)(direction.normalized * distance);

        bumpTween = Tween.Custom(0f, 1f, duration, t =>
        {       
            transform.position = start + offset * Mathf.Sin(t * Mathf.PI);
        }, Ease.OutQuad);
    }
     
    public void CancelBump(Vector3 restPosition)
    {
        if (!bumpTween.isAlive)
            return;

        bumpTween.Stop();
        transform.position = new Vector3(restPosition.x, restPosition.y, transform.position.z);
    }
}
