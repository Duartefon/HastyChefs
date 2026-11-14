using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class VitalBarUI : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Image ghostFill;
    [SerializeField] private float ghostDelay = 0.25f, ghostDuration = 0.4f;

    private Tween ghostTween;

    public void Set(float current, float max)
    {
        float t = max > 0f ? current / max : 0f;
        fill.fillAmount = t;

        if (ghostFill == null) return;
        ghostTween.Stop();
        if (Mathf.Approximately(t, ghostFill.fillAmount)) return; // already there: no tween needed
        if (t > ghostFill.fillAmount)
        {
            ghostFill.fillAmount = t;
            return;
        }
        else
        {
            ghostTween = Tween.UIFillAmount(ghostFill, t, ghostDuration, startDelay: ghostDelay);
        }
    }

    public void SetTint(Color c) => fill.color = c;
}
