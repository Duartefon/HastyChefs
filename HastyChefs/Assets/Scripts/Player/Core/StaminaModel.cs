using System;

namespace HastyChefs.Player.Core
{
    public readonly struct StaminaSettings
    {
        public readonly float Max;
        public readonly float SprintCostPerSecond;
        public readonly float RegenPerSecond;
        public readonly float RegenDelay;
        public readonly float ExhaustedRecoverAt;

        public StaminaSettings(float max, float sprintCostPerSecond, float regenPerSecond, float regenDelay, float exhaustedRecoverAt)
        {
            Max = max;
            SprintCostPerSecond = sprintCostPerSecond;
            RegenPerSecond = regenPerSecond;
            RegenDelay = regenDelay;
            ExhaustedRecoverAt = exhaustedRecoverAt;
        }
    }

    public class StaminaModel
    {
        private readonly StaminaSettings settings;
        private float regenTimer;

        public Vital Vital { get; }
        public bool IsExhausted { get; private set; }
        public bool CanSprint => !IsExhausted && !Vital.IsEmpty;

        public event Action<bool> ExhaustedChanged;

        public StaminaModel(StaminaSettings settings)
        {
            this.settings = settings;
            Vital = new Vital(settings.Max);
            Vital.Depleted += () => SetExhausted(true);
        }

        public void Tick(bool isSprinting, float dt)
        {
            if (isSprinting)
            {
                Vital.Decrease(settings.SprintCostPerSecond * dt);
                regenTimer = settings.RegenDelay;
            }
            else
            {
                regenTimer -= dt;
                if (regenTimer <= 0f)
                    Vital.Increase(settings.RegenPerSecond * dt);
            }

            if (IsExhausted && Vital.Normalized >= settings.ExhaustedRecoverAt)
                SetExhausted(false);
        }

        private void SetExhausted(bool value)
        {
            if (IsExhausted == value) return;
            IsExhausted = value;
            ExhaustedChanged?.Invoke(value);
        }
    }
}
