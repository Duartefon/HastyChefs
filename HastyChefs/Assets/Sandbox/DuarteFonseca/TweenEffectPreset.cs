using UnityEngine;
using PrimeTween;

public enum TweenEffectType
{
    ScaleUniform,
    FadeAlpha,
    UIAnchoredPositionX,
    UIAnchoredPositionY,
    UIRotationZ,        // Standard 2D UI tilt/spin (float)
    
    PunchScale,

    // Transform Operations
    Position,
    LocalPosition,
    RotationEuler,      // 3D rotation (Vector3)


 
    PunchRotation       // Great for UI button wobbles
}

[CreateAssetMenu(fileName = "NewTweenEffect", menuName = "UI/Tween Effect Preset")]
public class TweenEffectPreset : ScriptableObject
{
    [Header("Timeline Track Position")]
    [Tooltip("Point in seconds along the sequence when this effect triggers.")]
    [Range(0f, 5f)] public float startTime = 0f;

    [Header("Type")]
    public TweenEffectType effectType = TweenEffectType.ScaleUniform;

    [Header("Values")]
    public float startValue = 0f;
    public float endValue = 1f;
    public Vector3 punchStrength = new Vector3(0.1f, 0.1f, 0f);

    [Header("Timing & Curve")]
    [Range(0.01f, 5f)] public float duration = 0.35f;
    public Ease ease = Ease.OutBack;

    /// <summary>
    /// Configures initial visual state and generates the Tween without auto-playing.
    /// </summary>
    public Tween CreateTween(GameObject target, bool useUnscaledTime)
    {
        switch (effectType)
        {
            case TweenEffectType.ScaleUniform:
                target.transform.localScale = Vector3.one * startValue;
                return Tween.Scale(
                    target.transform, 
                    startValue: startValue, 
                    endValue: endValue, 
                    duration: duration, 
                    ease: ease, 
                    useUnscaledTime: useUnscaledTime);

            case TweenEffectType.FadeAlpha:
                if (target.TryGetComponent<CanvasGroup>(out var canvasGroup))
                {
                    canvasGroup.alpha = startValue;
                    return Tween.Alpha(
                        canvasGroup, 
                        startValue: startValue, 
                        endValue: endValue, 
                        duration: duration, 
                        ease: ease, 
                        useUnscaledTime: useUnscaledTime);
                }
                break;

            case TweenEffectType.UIAnchoredPositionX:
                if (target.TryGetComponent<RectTransform>(out var rectTransformX))
                {
                    rectTransformX.anchoredPosition = new Vector2(startValue, rectTransformX.anchoredPosition.y);
                    return Tween.UIAnchoredPositionX(
                        rectTransformX, 
                        startValue: startValue, 
                        endValue: endValue, 
                        duration: duration, 
                        ease: ease, 
                        useUnscaledTime: useUnscaledTime);
                }
                break;

            case TweenEffectType.UIAnchoredPositionY:
                if (target.TryGetComponent<RectTransform>(out var rectTransformY))
                {
                    rectTransformY.anchoredPosition = new Vector2(rectTransformY.anchoredPosition.x, startValue);
                    return Tween.UIAnchoredPositionY(
                        rectTransformY, 
                        startValue: startValue, 
                        endValue: endValue, 
                        duration: duration, 
                        ease: ease, 
                        useUnscaledTime: useUnscaledTime);
                }
                break;

            case TweenEffectType.PunchScale:
                return Tween.PunchScale(
                    target.transform, 
                    strength: punchStrength, 
                    duration: duration, 
                    frequency: 10, 
                    useUnscaledTime: useUnscaledTime);
        }

        return default;
    }
}