using UnityEngine;

public class UITheme : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;

    private ButtonStyle[] buttons;

    private void Awake()
    {
        if (cameraController == null)
            cameraController = FindFirstObjectByType<CameraController>();
              
        buttons = GetComponentsInChildren<ButtonStyle>(true);

        ApplyColour(cameraController.BackgroundColor);
    }

    private void OnEnable()
    {
        cameraController.BackgroundColorChanged += ApplyColour;
    }

    private void OnDisable()
    {
        cameraController.BackgroundColorChanged -= ApplyColour;
    }

    private void ApplyColour(Color colour)
    {
        colour.a = 1f;

        foreach (ButtonStyle button in buttons)
            button.SetHoveredTextColour(colour);
    }
}