using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class TweenEffectSequence : MonoBehaviour
{
    [Tooltip("Drag as many presets as you want here. They will merge into one sequence.")]
    [SerializeField] private List<TweenEffectPreset> effectPresets = new List<TweenEffectPreset>();

    [SerializeField] private bool useUnscaledTime = true;

    private Sequence activeSequence;

    private void OnEnable()
    {
        PlaySequence();
    }

    public void PlaySequence()
    {
        if (effectPresets == null || effectPresets.Count == 0) return;

        // Cancel previous animation to prevent overlap conflicts
        activeSequence.Stop();

        // 1. First pass: Apply all initial start values immediately so there are no frame pops
        foreach (var preset in effectPresets)
        {
            if (preset == null) continue;
            ApplyInitialState(preset);
        }

        // 2. Second pass: Build the multi-track timeline
        activeSequence = Sequence.Create(useUnscaledTime: useUnscaledTime);

        foreach (var preset in effectPresets)
        {
            if (preset == null) continue;

            Tween tween = preset.CreateTween(gameObject, useUnscaledTime);
            if (tween.isAlive)
            {
                // Inserts the tween at its designated start timestamp on the timeline
                activeSequence.Insert(preset.startTime, tween);
            }
        }
    }

    private void ApplyInitialState(TweenEffectPreset preset)
    {
        switch (preset.effectType)
        {
            case TweenEffectType.ScaleUniform:
                transform.localScale = Vector3.one * preset.startValue;
                break;

            case TweenEffectType.FadeAlpha:
                if (TryGetComponent<CanvasGroup>(out var canvasGroup))
                    canvasGroup.alpha = preset.startValue;
                break;

            case TweenEffectType.UIAnchoredPositionX:
                if (TryGetComponent<RectTransform>(out var rectTransformX))
                    rectTransformX.anchoredPosition = new Vector2(preset.startValue, rectTransformX.anchoredPosition.y);
                break;

            case TweenEffectType.UIAnchoredPositionY:
                if (TryGetComponent<RectTransform>(out var rectTransformY))
                    rectTransformY.anchoredPosition = new Vector2(rectTransformY.anchoredPosition.x, preset.startValue);
                break;
        }
    }

    private void OnDisable()
    {
        activeSequence.Stop();
    }
}