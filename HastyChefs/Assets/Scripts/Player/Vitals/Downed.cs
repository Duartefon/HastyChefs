using System;
using PurrNet;
using UnityEngine;
using HastyChefs.Player.Core;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class Downed : NetworkBehaviour, IInteractable
{
    [SerializeField] private Health health;
    [SerializeField] private PlayerStatsConfig config;
    [SerializeField] private SyncVar<bool> netDowned = new(false);

    public bool IsDowned => netDowned.value;

    public event Action<bool> Changed;

    private bool listeningToHealth;
    private bool lastRaised;

    private void Awake()
    {
        if (health == null) health = GetComponent<Health>();
        if (config == null && health != null) config = health.Config;
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        netDowned.onChanged += Raise;

        if (isServer && !listeningToHealth)
        {
            health.Died += OnDied;
            listeningToHealth = true;
        }

        Raise(netDowned.value);
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();
        netDowned.onChanged -= Raise;
        if (listeningToHealth)
        {
            health.Died -= OnDied;
            listeningToHealth = false;
        }
    }

    // ---------- Server ----------
    private void OnDied()
    {
        if (!isServer || IsDowned) return;
        netDowned.value = true;
        Raise(true);
    }

    public void ServerRevive()
    {
        if (!isServer || !IsDowned) return;
        netDowned.value = false;
        health.Heal(ReviveRules.RevivedHealth(config.maxHealth, config.reviveHealthFraction));
        Raise(false);
    }

    // ---------- Everyone ----------
    private void Raise(bool value)
    {
        if (value == lastRaised) return;
        lastRaised = value;
        Changed?.Invoke(value);
    }


    public string Prompt => "Hold E to revive";

    public bool CanInteract(PlayerInteractor who)
    {
        if (who == null) return false;
        var reviver = who.GetComponent<Downed>();
        return ReviveRules.CanRevive(
            targetIsDowned: IsDowned,
            reviverIsDowned: reviver != null && reviver.IsDowned,
            isSamePlayer: who.gameObject == gameObject);
    }

    public void ServerInteract(PlayerInteractor who) => ServerRevive();

#if UNITY_EDITOR //Temporary, just to test if the revive works 
    private void Update()
    {
        if (!isOwner) return;
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.jKey.wasPressedThisFrame)
            DebugReviveServerRpc();
    }

    [ServerRpc] private void DebugReviveServerRpc() => ServerRevive();
#endif
}
