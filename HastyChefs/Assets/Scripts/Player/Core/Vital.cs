using System;

namespace HastyChefs.Player.Core
{
    public class Vital
    {
        public float Current { get; private set; }
        public float Max { get; private set; }
        public float Normalized => Max > 0f ? Current / Max : 0f;
        public bool IsEmpty => Current <= 0f;
        public bool IsFull => Current >= Max;

        public event Action<float, float> Changed;
        public event Action Depleted;

        public Vital(float max, float? start = null)
        {
            if (max <= 0f) throw new ArgumentOutOfRangeException(nameof(max), "Max value must be greater than zero.");
            if (start.HasValue && (start < 0f || start > max)) throw new ArgumentOutOfRangeException(nameof(start), "Start value must be between 0 and Max.");
            Max = max;
            Current = start ?? max;
        }

        public float Decrease(float amount)
        {
            if (amount <= 0f) return 0f;

            float old = Current;
            Current -= amount;
            Current = Math.Clamp(Current, 0f, Max);

            if (old != Current)
            {
                Changed?.Invoke(Current, Max);
            }

            if (old > 0f && Current == 0f)
            {
                Depleted?.Invoke();
            }

            return old - Current;
        }

        public float Increase(float amount)
        {
            if (amount <= 0f) return 0f;

            float old = Current;
            Current += amount;
            Current = Math.Clamp(Current, 0f, Max);

            if (old != Current)
            {
                Changed?.Invoke(Current, Max);
            }

            return Current - old;
        }

        public void Set(float value)
        {
            float old = Current;
            Current = Math.Clamp(value, 0f, Max);

            if (old != Current)
            {
                Changed?.Invoke(Current, Max);
            }

            if (old > 0f && Current == 0f)
            {
                Depleted?.Invoke();
            }
        }

        public void SetMax(float newMax, bool keepRatio = true)
        {
            if (newMax <= 0f) throw new ArgumentOutOfRangeException(nameof(newMax), "Max value must be greater than zero.");

            float oldMax = Max;
            Max = newMax;

            if (keepRatio)
            {
                Current = Current / oldMax * Max;
            }
            else
            {
                Current = Math.Clamp(Current, 0f, Max);
            }

            Changed?.Invoke(Current, Max);
        }
    }
}