using UnityEngine;

[CreateAssetMenu(fileName = "HealEffectData", menuName = "ScriptableObjects/Effects/HealEffectData")]
public class HealEffectData : AbstractEffectData
{
    public override bool ApplyEffect(PlayerStats playerStats, float amount)
    {
        if (playerStats == null) return false;

        return playerStats.TryHeal(amount);
    }
}
