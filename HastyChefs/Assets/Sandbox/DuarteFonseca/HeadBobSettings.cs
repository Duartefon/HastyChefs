using UnityEngine;

[CreateAssetMenu(fileName = "HeadBobSettings", menuName = "Scriptable Objects/HeadBobSettings")]
public class HeadBobSettings : ScriptableObject
{
    [SerializeField] public float _toggleSpeed  = 3f; 
    [SerializeField] public float _stepFrequency = 2f; // How many full step cycles per second
    [SerializeField] public float _returnSpeed = 6f;
    [SerializeField] public float _amplitudeMultiplier = 1f;
    [Header("HeadBob Graphs")]
    [Tooltip("Vertical bounce over one full step cycle (X: 0 to 1, Y: vertical offset in meters)")]
    [SerializeField] public AnimationCurve _verticalCurve = new AnimationCurve(
        new Keyframe(0.0f, 0.0f),
        new Keyframe(0.25f, 0.05f),
        new Keyframe(0.5f, 0.0f),
        new Keyframe(0.75f, 0.05f),
        new Keyframe(1.0f, 0.0f)
    );

    [Tooltip("Horizontal sway over one full step cycle (X: 0 to 1, Y: horizontal offset in meters)")]
    [SerializeField] public AnimationCurve _horizontalCurve = new AnimationCurve(
        new Keyframe(0.0f, 0.0f),
        new Keyframe(0.25f, -0.04f),
        new Keyframe(0.5f, 0.0f),
        new Keyframe(0.75f, 0.04f),
        new Keyframe(1.0f, 0.0f)
    );
}
