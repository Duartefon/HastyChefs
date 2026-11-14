using System;
using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractor : NetworkBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float range = 2.5f;
    [SerializeField] private InputActionReference interactAction;

    public event Action<IInteractable> FocusChanged;
    public IInteractable Focused { get; private set; }

    private readonly RaycastHit[] hits = new RaycastHit[8];

    private void Awake()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>(true);
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        if (isOwner && interactAction != null) interactAction.action.Enable();
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();
        if (isOwner && interactAction != null) interactAction.action.Disable();
    }

    private void Update()
    {
        if (!isOwner || cam == null) return;

        SetFocus(FindTarget());

        if (Focused is NetworkIdentity target && interactAction != null && interactAction.action.WasPerformedThisFrame())
            InteractServerRpc(target);
    }

    private IInteractable FindTarget()
    {
        var ray = new Ray(cam.transform.position, cam.transform.forward);
        int count = Physics.RaycastNonAlloc(ray, hits, range, ~0, QueryTriggerInteraction.Collide);

        IInteractable best = null;
        float bestDistance = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            var hit = hits[i];
            if (hit.collider.transform.IsChildOf(transform)) continue;
            if (hit.distance >= bestDistance) continue;

            var candidate = hit.collider.GetComponentInParent<IInteractable>();
            if (candidate == null || !candidate.CanInteract(this)) continue;

            best = candidate;
            bestDistance = hit.distance;
        }
        return best;
    }

    private void SetFocus(IInteractable target)
    {
        if (ReferenceEquals(target, Focused)) return;
        Focused = target;
        FocusChanged?.Invoke(target);
    }

    [ServerRpc]
    private void InteractServerRpc(NetworkIdentity target)
    {
        if (target == null || target is not IInteractable interactable) return;
        if (!interactable.CanInteract(this)) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > range + 1f) return; // lag tolerance (+1f)

        interactable.ServerInteract(this);
    }
}
