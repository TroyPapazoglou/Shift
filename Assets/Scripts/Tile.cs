using UnityEngine;

public class Tile : GridVisual
{
    private int originalSortingOrder;

    public Vector2Int GridPosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    public void Initialize(Vector2Int position, TileType type, Sprite sprite, float spacing)
    {
        GridPosition = position;

        transform.localPosition = new Vector3(
            position.x * spacing,
            -position.y * spacing,
            0);

        spriteRenderer.sprite = sprite;
    }

    public void UpdatePosition(Vector2Int position, float spacing)
    {
        GridPosition = position;

        transform.localPosition = new Vector3(
            position.x * spacing,
            -position.y * spacing,
            0);
    }

    public void SetGridPosition(Vector2Int position)
    {
        GridPosition = position;
    }

    public void SetUnderneath()
    {
        spriteRenderer.sortingOrder = originalSortingOrder - 1;
    }

    public void RestoreSortingOrder()
    {
        spriteRenderer.sortingOrder = originalSortingOrder;
    }

    public override void AnimateTo(Vector3 destination, float duration, Vector3? wrapExit, Vector3? wrapEntry, System.Action onComplete = null)
    {
        SetUnderneath();

        base.AnimateTo(destination, duration, wrapExit, wrapEntry, () =>
        {
            RestoreSortingOrder();
            onComplete?.Invoke();
        });
    }
}
