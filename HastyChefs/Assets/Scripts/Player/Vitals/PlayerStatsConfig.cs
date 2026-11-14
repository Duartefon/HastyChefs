using UnityEngine;

[CreateAssetMenu(menuName = "HastyChefs/Player/Stats Config")]
public class PlayerStatsConfig : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float sprintCostPerSecond = 25f;
    public float regenPerSecond = 20f;
    public float regenDelay = 1f;
    [Range(0f, 1f)] public float exhaustedRecoverAt = 0.2f; //I guess this numeber is still not final according to the GDD

    [Header("Downed")]
    [Range(0f, 1f)] public float reviveHealthFraction = 0.3f; // revived players get back up with 30% health
}