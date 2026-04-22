using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffectData", menuName = "ScriptableObjects/Effects/DamageEffectData")]
public class DamageEffectData : AbstractEffectData
{

    public override bool ApplyEffect(PlayerStats playerStats, float amount)
    {
        if (playerStats == null) return false;

        playerStats.TakeDamage(amount);
        return true;
    }
}
