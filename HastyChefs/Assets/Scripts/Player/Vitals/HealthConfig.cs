using UnityEngine;

[CreateAssetMenu(menuName = "HastyChefs/Vitals/HealthConfig")]
public class HealthConfig : ScriptableObject
{
    [Header("Health")]
    [Min(1f)] public float maxHealth = 100f;
}