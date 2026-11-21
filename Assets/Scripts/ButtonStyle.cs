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
    [SerializeField] private Color hoveredBackgroundColour = new Color(0.92f, 0.82f, 0.55f, 1f);

    private Color hoveredTextColour = Color.white;

    private void Awake()
    {
        if(Camera.main != null)
        {
            hoveredTextColour = Camera.main.backgroundColor;
            hoveredTextColour.a = 1f;
        }
           

        UpdateStyle(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateStyle(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateStyle(false);
    }

    private void UpdateStyle(bool isHovered)
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
                ? hoveredTextColour
                : normalTextColour;
        }
    }
}
