using System;
using PurrNet;
using UnityEngine;
using HastyChefs.Player.Core;

public class Stamina : NetworkBehaviour
{
    [SerializeField] private PlayerStatsConfig config;
    [SerializeField] private SyncVar<bool> netExhausted = new(false, ownerAuth: true);

    private StaminaModel model;

    public Vital Vital => model.Vital;
    public bool IsExhausted => netExhausted.value;  //This is synced beacause we might want to have exaustion animations or sounds and each player would have to render that.
    public bool CanSprint => model.CanSprint;
    public event Action<float, float> Changed;
    public event Action<bool> ExhaustedChanged;

    private void Awake()
    {
        model = new StaminaModel(new StaminaSettings(
            config.maxStamina,
            config.sprintCostPerSecond,
            config.regenPerSecond,
            config.regenDelay,
            config.exhaustedRecoverAt));

        model.Vital.Changed += (current, max) => Changed?.Invoke(current, max);
        model.ExhaustedChanged += value => { if (isOwner) netExhausted.value = value; };
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        netExhausted.onChanged += value => ExhaustedChanged?.Invoke(value);
    }

    public void Tick(bool isSprinting, float dt)
    {
        if (!isOwner) return;
        model.Tick(isSprinting, dt);
    }
}