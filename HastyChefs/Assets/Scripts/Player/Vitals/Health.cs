using System;
using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;
using HastyChefs.Player.Core;

public class Health : NetworkBehaviour
{
    [SerializeField] private HealthConfig config;
    [SerializeField] private SyncVar<float> netHealth = new(100f);
    public Vital Vital { get; private set; }
    public event Action<float, float> Changed;
    public event Action Died;

    private void Awake()
    {
        Vital = new Vital(config.maxHealth);
        Vital.Changed += (current, max) => Changed?.Invoke(current, max);
        Vital.Depleted += () => Died?.Invoke();
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        if (isServer) netHealth.value = config.maxHealth;
        netHealth.onChanged += OnNetHealthChanged;
        Vital.Set(netHealth.value);
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();
        netHealth.onChanged -= OnNetHealthChanged;
    }

    private void OnNetHealthChanged(float value) => Vital.Set(value);

    //Server-side
    public void TakeDamage(float amount)
    {
        if (!isServer) { Debug.LogWarning("TakeDamage must run on the server", this); return; }
        Vital.Decrease(amount);
        netHealth.value = Vital.Current;
    }

    public void Heal(float amount)
    {
        if (!isServer) { Debug.LogWarning("Heal must run on the server", this); return; }
        Vital.Increase(amount);
        netHealth.value = Vital.Current;
    }

#if UNITY_EDITOR //Temporary, just to test if the damage works
    private void Update()
    {
        if (!isOwner) return;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.kKey.wasPressedThisFrame)
            DebugDamageServerRpc(10f);
    }

    [ServerRpc] private void DebugDamageServerRpc(float amount) => TakeDamage(amount);
#endif
}