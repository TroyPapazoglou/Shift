using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Swipe")]
    [Tooltip("Minimum swipe length, as a fraction of the shorter screen side.")]
    [SerializeField, Range(0.02f, 0.3f)] private float minSwipeFraction = 0.06f;

    [Tooltip("How much longer the main axis must be than the other. Higher = ignores diagonal swipes.")]
    [SerializeField, Min(1f)] private float minAxisDominance = 1.2f;

    private PlayerInput playerInput;

    private Vector2 swipeStart;
    private bool swipeTracking;

    private static readonly List<RaycastResult> uiHits = new List<RaycastResult>();

    public event Action<Vector2Int> MoveRequested;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Gameplay.Enable();
    }

    private void OnDestroy()
    {
        playerInput.Dispose();
    }

    private void OnEnable()
    {
        playerInput.Gameplay.Shift.performed += OnShiftPerformed;
        playerInput.Gameplay.Shift.Enable();

        playerInput.Gameplay.SwipePress.started += OnSwipeStarted;
        playerInput.Gameplay.SwipePress.canceled += OnSwipeEnded;
        playerInput.Gameplay.SwipePress.Enable();
        playerInput.Gameplay.PointerPosition.Enable();
    }

    private void OnDisable()
    {
        playerInput.Gameplay.Shift.performed -= OnShiftPerformed;
        playerInput.Gameplay.Shift.Disable();

        playerInput.Gameplay.SwipePress.started -= OnSwipeStarted;
        playerInput.Gameplay.SwipePress.canceled -= OnSwipeEnded;
        playerInput.Gameplay.SwipePress.Disable();
        playerInput.Gameplay.PointerPosition.Disable();

        swipeTracking = false;
    }

    private void OnShiftPerformed(InputAction.CallbackContext context)
    {
        RaiseMove(context.ReadValue<Vector2>());
    }

    private void OnSwipeStarted(InputAction.CallbackContext context)
    {
        swipeStart = playerInput.Gameplay.PointerPosition.ReadValue<Vector2>();
      
        swipeTracking = !IsPointerOverUI(swipeStart);
    }

    private void OnSwipeEnded(InputAction.CallbackContext context)
    {
        if (!swipeTracking)
            return;

        swipeTracking = false;

        Vector2 delta = playerInput.Gameplay.PointerPosition.ReadValue<Vector2>() - swipeStart;

        float minDistance = Mathf.Min(Screen.width, Screen.height) * minSwipeFraction;

        if (delta.magnitude < minDistance)
            return; 

        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);
               
        if (Mathf.Max(absX, absY) < Mathf.Min(absX, absY) * minAxisDominance)
            return;

        RaiseMove(delta);
    }

    private static bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        uiHits.Clear();
        EventSystem.current.RaycastAll(data, uiHits);

        return uiHits.Count > 0;
    }   
     
    private void RaiseMove(Vector2 input)
    {
        Vector2Int direction;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            direction = input.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            direction = input.y > 0 ? Vector2Int.down : Vector2Int.up;
        }

        MoveRequested?.Invoke(direction);
    }
}
