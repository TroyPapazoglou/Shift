using UnityEngine;

public class PlayerVisual : GridVisual
{
    [SerializeField] private int sortingOrderOffset = 1;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer.sortingOrder += sortingOrderOffset;
    }
}
