using UnityEngine;

[CreateAssetMenu(fileName = "StrengthEffectData", menuName = "ScriptableObjects/Effects/StrengthEffectData")]
public class StrengthEffectData : AbstractEffectData
{

    public override void ApplyEffect(PlayerStats playerStats, float amount)
    {
        if (playerStats == null) return;

        playerStats.UpdateStrength(amount);
    }
}
