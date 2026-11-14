using PrimeTween;
using UnityEngine;


public class DownedVisual : MonoBehaviour
{
    [SerializeField] private Downed downed;
    [SerializeField] private Transform body;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 bodyDownedPosition = new Vector3(0f, 0.5f, -1.1f);
    [SerializeField] private Vector3 bodyDownedEuler = new Vector3(90f, 0f, 0f);
    [SerializeField] private float cameraDrop = 1.4f;
    [SerializeField] private float duration = 0.3f;

    private Vector3 bodyUpPosition, cameraUpPosition;
    private Quaternion bodyUpRotation;

    private void Awake()
    {
        if (downed == null) downed = GetComponent<Downed>();
        if (body == null) body = transform.Find("Body");
        if (cameraTransform == null)
        {
            var cam = GetComponentInChildren<Camera>(true);
            if (cam != null) cameraTransform = cam.transform;
        }

        bodyUpPosition = body.localPosition;
        bodyUpRotation = body.localRotation;
        if (cameraTransform != null) cameraUpPosition = cameraTransform.localPosition;
    }

    private void OnEnable() => downed.Changed += Apply;
    private void OnDisable() => downed.Changed -= Apply;

    private void Apply(bool isDowned)
    {
        Tween.StopAll(body);
        Tween.LocalPosition(body, isDowned ? bodyDownedPosition : bodyUpPosition, duration);
        Tween.LocalRotation(body, isDowned ? Quaternion.Euler(bodyDownedEuler) : bodyUpRotation, duration);

        if (cameraTransform != null)
        {
            Tween.StopAll(cameraTransform);
            Vector3 target = isDowned ? cameraUpPosition + Vector3.down * cameraDrop : cameraUpPosition;
            Tween.LocalPosition(cameraTransform, target, duration);
        }
    }
}
