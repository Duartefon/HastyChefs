using System;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

[RequireComponent(typeof(CanvasGroup))]
public class TweenEffectSequence : MonoBehaviour, IAnimatedView
{
    [Header("Presets")]
    [Tooltip("Presets played when the view opens.")]
    [SerializeField] private List<TweenEffectPreset> openPresets = new List<TweenEffectPreset>();

    [Tooltip("Presets played when the view closes (optional - can rely on fallback fade).")]
    [SerializeField] private List<TweenEffectPreset> closePresets = new List<TweenEffectPreset>();

    [Header("Fallback Settings")]
    [Tooltip("Duration of the fallback alpha fade if no closePresets are specified.")]
    [SerializeField] private float defaultFadeDuration = 0.2f;
    [SerializeField] private bool useUnscaledTime = true;

    [SerializeField] private CanvasGroup canvasGroup;
    private Sequence activeSequence;

    public bool IsOpen { get; private set; }



    //se isto funcionar passar para um TweenEffectSequenceUI
    private void Start()
    {
        Show();
    }
    /// <summary>
    /// Opens the menu, restores interactivity, and runs opening animations.
    /// </summary>
    public void Show()
    {
        
        IsOpen = true;

        gameObject.SetActive(true); // to delete after testing 

        activeSequence.Stop();

        // Re-enable clicks and input //turn into a fuction
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        
        PlaySequence(openPresets, onComplete: null);
    }

    /// <summary>
    /// Disables input immediately, fades out/animates close, then deactivates GameObject.
    /// </summary>
    public void Hide()
    {
        Debug.Log("O gameobject é: " + gameObject.name);
        if (!gameObject.activeInHierarchy || !IsOpen) //too delete
        {
            IsOpen = false;
            return;
        }

        IsOpen = false;

        // 1. Instantly block UI clicks so the player can't spam buttons while it closes //function with bool value
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        activeSequence.Stop();

        // 2. If you defined custom close presets, play them // probably to delete too, test with no effects given in the inspector
        if (closePresets != null && closePresets.Count > 0)
        {
            PlaySequence(closePresets, onComplete: () => gameObject.SetActive(false));
        }
        else
        {
            // 3. Fallback: Quick, clean CanvasGroup alpha fade to 0
            activeSequence = Sequence.Create(useUnscaledTime: useUnscaledTime)
                .Group(Tween.Alpha(canvasGroup, endValue: 0f, duration: defaultFadeDuration, ease: Ease.InQuad,
                    useUnscaledTime: useUnscaledTime))
           .ChainCallback(() => gameObject.SetActive(false));
        }
    }

    private void PlaySequence(List<TweenEffectPreset> presets, Action onComplete)
    {

        activeSequence = Sequence.Create(useUnscaledTime: useUnscaledTime);

        // Build track
        foreach (var preset in presets)
        {
           // ApplyInitialState(preset);
            Tween tween = preset.CreateTween(gameObject, useUnscaledTime);
            if (tween.isAlive)
            {
                activeSequence.Insert(preset.startTime, tween);
                Debug.Log("ActiveSequnece" + activeSequence + "Tween inserted at: " + preset.startTime);
                
            }
            
        }

        if (onComplete != null)
        {
            activeSequence.ChainCallback(onComplete);
        }
    }

    private void ApplyInitialState(TweenEffectPreset preset)
    {
        switch (preset.effectType)
        {
            case TweenEffectType.FadeAlpha:
                canvasGroup.alpha = preset.startValue;
                break;

            case TweenEffectType.ScaleUniform:
                transform.localScale = Vector3.one * preset.startValue;
                break;

            case TweenEffectType.UIAnchoredPositionX:
                if (TryGetComponent<RectTransform>(out var rectTransformX))
                    rectTransformX.anchoredPosition = new Vector2(preset.startValue, rectTransformX.anchoredPosition.y);
                break;

            case TweenEffectType.UIAnchoredPositionY:
                if (TryGetComponent<RectTransform>(out var rectTransformY))
                    rectTransformY.anchoredPosition = new Vector2(rectTransformY.anchoredPosition.x, preset.startValue);
                break;
            case TweenEffectType.UIRotationZ:
                transform.localEulerAngles = new Vector3(0f, 0f, preset.startValue);
                break;

            case TweenEffectType.RotationEuler:
                transform.localEulerAngles = preset.startVector3;
                break;
        }
    }

    private void OnDisable()
    {
        // Safety cleanup if turned off externally
        activeSequence.Stop();
    }
}