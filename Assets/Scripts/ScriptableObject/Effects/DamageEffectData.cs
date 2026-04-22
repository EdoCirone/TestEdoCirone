using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffectData", menuName = "ScriptableObjects/Effects/DamageEffectData")]
public class DamageEffectData : AbstractEffectData
{

    public override void ApplyEffect(PlayerStats playerStats, float amount)
    {
        if (playerStats == null) return;

        playerStats.TakeDamage(amount);
    }
}
