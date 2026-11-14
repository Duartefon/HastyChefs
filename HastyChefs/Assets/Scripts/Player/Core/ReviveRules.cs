using System;

namespace HastyChefs.Player.Core
{
    public static class ReviveRules
    {        public static bool CanRevive(bool targetIsDowned, bool reviverIsDowned, bool isSamePlayer)
        {
            if (!targetIsDowned) return false;
            if (isSamePlayer) return false;    
            return !reviverIsDowned;
        }

        public static float RevivedHealth(float maxHealth, float reviveHealthFraction)
        {
            Math.Clamp(reviveHealthFraction, 0f, 1f);
            return maxHealth * reviveHealthFraction;
        }
    }
}
