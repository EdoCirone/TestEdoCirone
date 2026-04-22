using UnityEngine;

[CreateAssetMenu(fileName = "SpeedEffectData", menuName = "ScriptableObjects/Effects/SpeedEffectData")]
public class SpeedEffectData : AbstractEffectData
{

    public override bool ApplyEffect(PlayerStats playerStats, float amount)
    {
        if (playerStats == null) return false;

        playerStats.UpdateSpeed(amount);
        return true;
    }
}
