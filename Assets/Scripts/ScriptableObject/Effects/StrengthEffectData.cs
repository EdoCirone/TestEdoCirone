using UnityEngine;

[CreateAssetMenu(fileName = "StrengthEffectData", menuName = "ScriptableObjects/Effects/StrengthEffectData")]
public class StrengthEffectData : AbstractEffectData
{

    public override bool ApplyEffect(PlayerStats playerStats, float amount)
    {
        if (playerStats == null) return false;

        playerStats.UpdateStrength(amount);
        return true;
    }
}
