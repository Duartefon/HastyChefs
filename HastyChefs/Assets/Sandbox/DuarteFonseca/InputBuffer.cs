using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffer
{
    private readonly InputAction _action;
    private readonly float _bufferDuration;
    private float _lastPressedTime = -999f;

    public InputBuffer(InputAction action, float bufferDuration = 0.15f)
    {
        _action = action;
        _bufferDuration = bufferDuration;
    }

    // Call this every render frame in Update()
    public void Tick()
    {
        if (_action.WasPressedThisFrame())
        {
            _lastPressedTime = Time.time;
        }
    }

    // True if the button was pressed within the last _bufferDuration seconds
    public bool IsBuffered => (Time.time - _lastPressedTime) <= _bufferDuration;

    // Call this once the action actually executes (e.g., when the jump fires)
    public void Consume()
    {
        _lastPressedTime = -999f;
    }
}