using PrimeTween;
using UnityEngine;

public class UIIntro : MonoBehaviour
{
    [Tooltip("In the order they should appear: title, best, moves, then the buttons.")]
    [SerializeField] private RectTransform[] elements;

    [SerializeField] private float startDelay = 0.4f;
    [SerializeField] private float stagger = 0.15f;
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup[] groups;

    private void Awake()
    {
        groups = new CanvasGroup[elements.Length];

        for (int i = 0; i < elements.Length; i++)
        {
            CanvasGroup group = elements[i].GetComponent<CanvasGroup>();

            if (group == null)
                group = elements[i].gameObject.AddComponent<CanvasGroup>();
                      
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            groups[i] = group;
        }
    }

    public void Play()
    {
        for (int i = 0; i < groups.Length; i++)
        {
            CanvasGroup group = groups[i];

            Tween.Alpha(group, 1f, fadeDuration, Ease.OutCubic, startDelay: startDelay + i * stagger)
                .OnComplete(() =>
                {
                    group.interactable = true;
                    group.blocksRaycasts = true;
                });
        }
    }
}