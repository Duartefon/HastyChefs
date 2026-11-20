using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _enable = true;


    [Header("References")]
    [SerializeField] private Transform _camera = null;
    [SerializeField] private HeadBobSettings _headBobSettings;
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
        var _toggleSpeed = _headBobSettings._toggleSpeed;
        var _stepFrequency = _headBobSettings._stepFrequency;
        
        
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
        var _horizontalCurve = _headBobSettings._horizontalCurve;
        var _verticalCurve = _headBobSettings._verticalCurve;
        var _amplitudeMultiplier = _headBobSettings._amplitudeMultiplier;
        
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
        var _returnSpeed = _headBobSettings._returnSpeed;
        
        
        _cycleProgress = 0f;
        if ((_camera.localPosition - _startPos).sqrMagnitude < 0.00001f)
        {
            _camera.localPosition = _startPos;
            return;
        }

        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, _returnSpeed * Time.deltaTime);
    }
}