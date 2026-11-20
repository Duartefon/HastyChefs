using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _enable = true;
    [SerializeField] private float _toggleSpeed = 3f;
    [SerializeField] private float _stepFrequency = 2f; // How many full step cycles per second
    [SerializeField] private float _returnSpeed = 6f;

    [Header("HeadBob Graphs")]
    [Tooltip("Vertical bounce over one full step cycle (X: 0 to 1, Y: vertical offset in meters)")]
    [SerializeField] private AnimationCurve _verticalCurve = new AnimationCurve(
        new Keyframe(0.0f, 0.0f),
        new Keyframe(0.25f, 0.05f),
        new Keyframe(0.5f, 0.0f),
        new Keyframe(0.75f, 0.05f),
        new Keyframe(1.0f, 0.0f)
    );

    [Tooltip("Horizontal sway over one full step cycle (X: 0 to 1, Y: horizontal offset in meters)")]
    [SerializeField] private AnimationCurve _horizontalCurve = new AnimationCurve(
        new Keyframe(0.0f, 0.0f),
        new Keyframe(0.25f, -0.04f),
        new Keyframe(0.5f, 0.0f),
        new Keyframe(0.75f, 0.04f),
        new Keyframe(1.0f, 0.0f)
    );

    [SerializeField] private float _amplitudeMultiplier = 1f;

    [Header("References")]
    [SerializeField] private Transform _camera = null;

    private Vector3 _startPos;
    private Rigidbody _playerBody;
    private Sandbox.DuarteFonseca.PlayerController _playerController;
    private float _cycleProgress;

    private void Awake()
    {
        _startPos = _camera.localPosition;
        _playerBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _playerController = GetComponent<Sandbox.DuarteFonseca.PlayerController>();
    }

    private void Update()
    {
        if (!_enable) return;

        CheckMotion();
    }

    private void CheckMotion()
    {
        float speed = new Vector3(_playerBody.linearVelocity.x, 0f, _playerBody.linearVelocity.z).magnitude;
        bool isGrounded = _playerController.IsGrounded;

        if (speed < _toggleSpeed || !isGrounded)
        {
            ResetPosition();
        }
        else
        {
            // Advance normalized cycle time (0 -> 1) and wrap cleanly
            _cycleProgress = (_cycleProgress + Time.deltaTime * _stepFrequency) % 1f;
            PlayMotion(FootStepMotion(_cycleProgress));
        }
    }

    private Vector3 FootStepMotion(float time)
    {
        float posX = _horizontalCurve.Evaluate(time) * _amplitudeMultiplier;
        float posY = _verticalCurve.Evaluate(time) * _amplitudeMultiplier;
        return new Vector3(posX, posY, 0f);
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition = Vector3.Lerp(
            _camera.localPosition,
            _startPos + motion,
            Time.deltaTime * 15f
        );
    }

    private void ResetPosition()
    {
        _cycleProgress = 0f;
        if ((_camera.localPosition - _startPos).sqrMagnitude < 0.00001f)
        {
            _camera.localPosition = _startPos;
            return;
        }

        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, _returnSpeed * Time.deltaTime);
    }
}