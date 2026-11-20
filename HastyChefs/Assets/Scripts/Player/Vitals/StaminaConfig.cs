using UnityEngine;

[CreateAssetMenu(menuName = "HastyChefs/Vitals/StaminaConfig")]
public class StaminaConfig : ScriptableObject
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float sprintCostPerSecond = 25f;
    public float regenPerSecond = 20f;
    public float regenDelay = 1f;
    [Range(0f, 1f)] public float exhaustedRecoverAt = 0.2f; //I guess this numeber is still not final according to the GDD
}
