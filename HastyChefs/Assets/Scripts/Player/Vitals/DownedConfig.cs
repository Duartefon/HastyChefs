using UnityEngine;

[CreateAssetMenu(menuName = "HastyChefs/Vitals/DownedConfig")]
public class DownedConfig : ScriptableObject
{
    [Header("Downed")]
    [Range(0f, 1f)] public float reviveHealthFraction = 0.3f; // revived players get back up with 30% health
}
