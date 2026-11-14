using PurrNet;
using TMPro;
using UnityEngine;

public class PlayerHUD : NetworkBehaviour
{
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private Health health;
    [SerializeField] private Stamina stamina;
    [SerializeField] private VitalBarUI healthBar, staminaBar;
    [SerializeField] private Color normalColor = Color.white, exhaustedColor = Color.red;

    [Header("Downed / interaction")]
    [SerializeField] private Downed downed;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private GameObject downedLabel;  // "You are down" text, hidden by default
    [SerializeField] private TMP_Text promptLabel;    // "Hold E to revive" text near the crosshair

    protected override void OnSpawned()
    {
        base.OnSpawned();
        if (!isOwner) { hudRoot.SetActive(false); return; }
        health.Changed += healthBar.Set;
        stamina.Changed += staminaBar.Set;
        stamina.ExhaustedChanged += OnExhaustedChanged;

        healthBar.Set(health.Vital.Current, health.Vital.Max);
        staminaBar.Set(stamina.Vital.Current, stamina.Vital.Max);
        OnExhaustedChanged(stamina.IsExhausted);

        if (downed != null)
        {
            downed.Changed += OnDownedChanged;
            OnDownedChanged(downed.IsDowned);
        }
        if (interactor != null)
        {
            interactor.FocusChanged += OnFocusChanged;
            OnFocusChanged(interactor.Focused);
        }
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();
        if (!isOwner) return;
        health.Changed -= healthBar.Set;
        stamina.Changed -= staminaBar.Set;
        stamina.ExhaustedChanged -= OnExhaustedChanged;
        if (downed != null) downed.Changed -= OnDownedChanged;
        if (interactor != null) interactor.FocusChanged -= OnFocusChanged;
    }

    private void OnDownedChanged(bool isDowned)
    {
        if (downedLabel != null) downedLabel.SetActive(isDowned);
    }

    private void OnFocusChanged(IInteractable target)
    {
        if (promptLabel == null) return;
        promptLabel.gameObject.SetActive(target != null);
        if (target != null) promptLabel.text = target.Prompt;
    }

    private void OnExhaustedChanged(bool isExhausted) =>
        staminaBar.SetTint(isExhausted ? exhaustedColor : normalColor);
}