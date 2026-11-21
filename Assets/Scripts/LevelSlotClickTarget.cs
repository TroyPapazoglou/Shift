using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelSlotClickTarget : MonoBehaviour, IPointerClickHandler
{
    private int levelIndex;
    private Action<int> onClicked;

    public void Initialize(int index, Action<int> callback)
    {
        levelIndex = index;
        onClicked = callback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClicked?.Invoke(levelIndex);
    }
}