using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private CameraController cameraController;

    [Header("Colours")]
    [SerializeField] private Color normalTextColour = Color.white;
    [SerializeField] private Color normalBackgroundColour = new Color(1f, 1f, 1f, 0f);
    [SerializeField] private Color hoveredBackgroundColour = new Color(0.92f, 0.82f, 0.55f, 1f);

    private bool isHovered;

    private void Awake()
    {
        if (cameraController == null && Camera.main != null)
            cameraController = Camera.main.GetComponent<CameraController>();

        UpdateStyle();
    }

    private void OnEnable()
    {
        if (cameraController != null)
            cameraController.BackgroundColorChanged += OnBackgroundColorChanged;
    }

    private void OnDisable()
    {
        if (cameraController != null)
            cameraController.BackgroundColorChanged -= OnBackgroundColorChanged;

        isHovered = false;
        UpdateStyle();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateStyle();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateStyle();
    }
      
    private void OnBackgroundColorChanged(Color color)
    {
        if (isHovered)
            UpdateStyle();
    }

    private void UpdateStyle()
    {
        if (background != null)
        {
            background.color = isHovered
                ? hoveredBackgroundColour
                : normalBackgroundColour;
        }

        if (buttonText != null)
        {
            buttonText.color = isHovered
                ? GetHoveredTextColour()
                : normalTextColour;
        }
    }
      
    private Color GetHoveredTextColour()
    {
        Color colour = cameraController != null
            ? cameraController.BackgroundColor
            : Color.black;

        colour.a = 1f;
        return colour;
    }
}