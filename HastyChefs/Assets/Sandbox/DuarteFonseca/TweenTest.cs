using System;
using UnityEngine;
using PrimeTween;
public class TweenTest : MonoBehaviour
{


 
    [SerializeField] private RectTransform menuPanel;

    [Header("Visual Tween Settings")]
    // Automatically renders duration, ease curve, cycles, startDelay in the Inspector
    [SerializeField] private TweenSettings<Vector2> slideSettings;
    [SerializeField] private TweenSettings<float> fadeSettings;

    [SerializeField] private CanvasGroup canvasGroup;


    private void Start()
    {
        Open();
    }

    public void Open()
    {
        Sequence.Create(useUnscaledTime: true)
            // Passes Inspector-configured duration, curve, and values directly
            .Group(Tween.UIAnchoredPosition(menuPanel, slideSettings))
            .Group(Tween.Alpha(canvasGroup, fadeSettings));
    }

    public void Close()
    {
        // Reverse direction using the same visual setup
        Sequence.Create(useUnscaledTime: true)
            .Group(Tween.UIAnchoredPosition(menuPanel, slideSettings.WithDirection(toEndValue: false)))
            .Group(Tween.Alpha(canvasGroup, fadeSettings.WithDirection(toEndValue: false)));
    }

}
