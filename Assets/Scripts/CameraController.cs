using PrimeTween;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [Header("UI the board must stay clear of")]
    [SerializeField] private RectTransform topUI;     
    [SerializeField] private RectTransform bottomUI; 
    [SerializeField, Range(0f, 0.1f)] private float extraMargin = 0.02f; 

    public Camera Cam => cam;

    private readonly Vector3[] corners = new Vector3[4];

    private Bounds lastBounds;
    private float lastPadding;
    private bool hasTarget;

    private int lastScreenWidth;
    private int lastScreenHeight;
    private int refitCountdown;

    private Tween positionTween;
    private Tween sizeTween;
    private Tween backgroundTween;

    private void Awake()
    {
        if (cam == null)
            Debug.LogError("cam not assigned to CameraController.");

        cam.clearFlags = CameraClearFlags.SolidColor;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private void Update()
    {       
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            refitCountdown = 3; 
        }

        if (refitCountdown > 0 && --refitCountdown == 0 && hasTarget)
            SnapToBounds(lastBounds, lastPadding);
    }

    public void SnapToBounds(Bounds bounds, float padding)
    {
        Remember(bounds, padding);
        StopTweens();

        ComputeView(bounds, padding, out Vector3 position, out float size);
        cam.transform.position = position;
        cam.orthographicSize = size;
    }

    public void FocusBounds(Bounds bounds, float padding, float duration)
    {
        Remember(bounds, padding);
        StopTweens();

        ComputeView(bounds, padding, out Vector3 position, out float size);
        positionTween = Tween.Position(cam.transform, position, duration, Ease.InOutCubic);
        sizeTween = Tween.CameraOrthographicSize(cam, size, duration, Ease.InOutCubic);
    }

    private void Remember(Bounds bounds, float padding)
    {
        lastBounds = bounds;
        lastPadding = padding;
        hasTarget = true;
    }

    private void StopTweens()
    {
        if (positionTween.isAlive) positionTween.Stop();
        if (sizeTween.isAlive) sizeTween.Stop();
    }
        
    private void ComputeView(Bounds bounds, float padding, out Vector3 position, out float size)
    {
        float top = GetTopMargin();
        float bottom = GetBottomMargin();
        float play = Mathf.Max(0.2f, 1f - top - bottom); 

        float sizeForHeight = (bounds.extents.y + padding) / play;
        float sizeForWidth = (bounds.extents.x + padding) / cam.aspect;
        size = Mathf.Max(sizeForHeight, sizeForWidth);
              
        float playCenter = bottom + play / 2f;
        float yOffset = (0.5f - playCenter) * 2f * size;

        position = new Vector3(
            bounds.center.x,
            bounds.center.y + yOffset,
            cam.transform.position.z);
    }

    private float GetTopMargin()
    {
        if (topUI == null)
            return 0f;

        GetScreenYRange(topUI, out float minY, out _);
        return Mathf.Clamp01((Screen.height - minY) / Screen.height) + extraMargin;
    }

    private float GetBottomMargin()
    {
        if (bottomUI == null)
            return 0f;

        GetScreenYRange(bottomUI, out _, out float maxY);
        return Mathf.Clamp01(maxY / Screen.height) + extraMargin;
    }

    private void GetScreenYRange(RectTransform rect, out float minY, out float maxY)
    {
        Canvas canvas = rect.GetComponentInParent<Canvas>();

        Camera uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;

        rect.GetWorldCorners(corners);

        minY = float.MaxValue;
        maxY = float.MinValue;

        for (int i = 0; i < 4; i++)
        {
            float y = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[i]).y;
            minY = Mathf.Min(minY, y);
            maxY = Mathf.Max(maxY, y);
        }
    }

    public void SetBackgroundColor(Color color)
    {
        if (backgroundTween.isAlive) backgroundTween.Stop();

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = color;
    }

    public void FadeBackgroundColor(Color color, float duration)
    {
        if (backgroundTween.isAlive) backgroundTween.Stop();

        cam.clearFlags = CameraClearFlags.SolidColor;
        backgroundTween = Tween.CameraBackgroundColor(cam, color, duration, Ease.InOutSine);
    }
}