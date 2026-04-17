using UnityEngine;

[CreateAssetMenu(fileName = "HealEffectData", menuName = "ScriptableObjects/Effects/HealEffectData")]
public class HealEffectData : AbstractEffectData
{
    public override void ApplyEffect(PlayerStats playerStats, float amount)
    {
        if (playerStats == null) return;

        playerStats.Heal(amount);
    }
}
