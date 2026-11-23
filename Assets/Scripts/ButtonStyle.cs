using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Colours")]
    [SerializeField] private Color normalTextColour = Color.white;
    [SerializeField] private Color normalBackgroundColour = new Color(1f, 1f, 1f, 0f);
    [SerializeField] private Color hoveredBackgroundColour = Color.white;

    private Color hoveredTextColour = Color.black;
    private bool isHovered;

    private void Awake()
    {
        UpdateStyle();
    }

    private void OnDisable()
    {
        isHovered = false;
        UpdateStyle();
    }
     
    public void SetHoveredTextColour(Color colour)
    {
        hoveredTextColour = colour;

        if (isHovered)
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

    private void UpdateStyle()
    {
        if (background != null)
            background.color = isHovered ? hoveredBackgroundColour : normalBackgroundColour;

        if (buttonText != null)
            buttonText.color = isHovered ? hoveredTextColour : normalTextColour;
    }
}